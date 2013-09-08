using System;
using System.Linq;
using System.Net;
using System.Threading;
using Sodao.FastSocket.Client;
using Sodao.FastSocket.SocketBase;
using Sodao.FastSocket.SocketBase.Utils;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper server pool
    /// </summary>
    public sealed class ZookServerPool : IServerPool
    {
        #region Private Members
        private ZookClient _zkClient = null;
        private int _sessionTimeout;

        private EndPoint[] _serverlist = null;//zk集群服务器地址列表
        private int _hostAcquireTimes = 0;

        private int _protocolVersion = 0;//zk协议版本
        internal int _negotiatedSessionTimeout;//zk协商的timeout，毫秒数
        private long _sessionID = 0L;//sessionID
        private byte[] _sessionPassword = new byte[16];//session password

        private IConnection _currConnection = null;
        #endregion

        #region IServerPool Members
        /// <summary>
        /// connected
        /// </summary>
        public event Action<string, IConnection> Connected;
        /// <summary>
        /// serverAvailable
        /// </summary>
        public event Action ServerAvailable;

        /// <summary>
        /// Acquire
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public IConnection Acquire(byte[] hash)
        {
            return this.Acquire();
        }
        /// <summary>
        /// Acquire
        /// </summary>
        /// <returns></returns>
        public IConnection Acquire()
        {
            return this._currConnection;
        }
        /// <summary>
        /// GetAllNodeNames
        /// </summary>
        /// <returns></returns>
        public string[] GetAllNodeNames()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// TryRegisterNode
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public bool TryRegisterNode(string name, EndPoint endPoint)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// UnRegisterNode
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UnRegisterNode(string name)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// set options
        /// </summary>
        /// <param name="zkClient"></param>
        /// <param name="connectionString"></param>
        /// <param name="sessionTimeout"></param>
        /// <exception cref="ArgumentNullException">zkClient is null</exception>
        /// <exception cref="ArgumentNullException">connectionString is null or empty.</exception>
        internal void SetOptions(ZookClient zkClient, string connectionString, int sessionTimeout)
        {
            if (zkClient == null) throw new ArgumentNullException("zkClient");
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");

            this._zkClient = zkClient;
            this._sessionTimeout = sessionTimeout;
            this._serverlist = connectionString
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .OrderBy(c => Guid.NewGuid())
                .Select(c => new IPEndPoint(IPAddress.Parse(c.Substring(0, c.IndexOf(":"))), int.Parse(c.Substring(c.IndexOf(":") + 1))))
                .ToArray();

            this.CreateSocket();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// create socket connection
        /// </summary>
        private void CreateSocket()
        {
            var endPoint = this._serverlist[(Interlocked.Increment(ref this._hostAcquireTimes) & 0x7fffffff) % this._serverlist.Length];
            SocketConnector.BeginConnect(endPoint, this._zkClient, connection =>
            {
                if (connection == null) { TaskEx.Delay(new Random().Next(500, 1500), this.CreateSocket); return; }

                connection.Disconnected += this.OnDisconnected;
                this.FireConnected(endPoint.ToString(), connection);
                this.ConnectToZookeeper(connection);
            });
        }
        /// <summary>
        /// connect to zookeeper
        /// </summary>
        /// <param name="connection"></param>
        private void ConnectToZookeeper(IConnection connection)
        {
            var connectRequest = new Data.ConnectRequest(this._protocolVersion,
                this._zkClient._lastZxid,
                this._sessionTimeout,
                this._sessionID,
                this._sessionPassword);

            var request = new Request<ZookResponse>(this._zkClient.NextRequestSeqID(), "connect",
                Utils.Marshaller.Serialize(connectRequest, true),
                ex => connection.BeginDisconnect(ex),
                response =>
                {
                    Data.ConnectResponse result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.ConnectResponse>(response.Payload); }
                    catch (Exception ex) { connection.BeginDisconnect(ex); return; }

                    if (result.SessionTimeOut <= 0)//session expired
                    {
                        this._protocolVersion = 0;
                        this._sessionID = 0;
                        this._negotiatedSessionTimeout = 0;
                        this._sessionPassword = new byte[16];

                        connection.BeginDisconnect(new ApplicationException("zookeeper session expired"));
                        return;
                    }

                    this._protocolVersion = result.ProtocolVersion;
                    this._negotiatedSessionTimeout = result.SessionTimeOut;
                    this._sessionID = result.SessionID;
                    this._sessionPassword = result.SessionPassword;

                    this._currConnection = connection;
                    this.FireServerAvailable();
                });

            connection.UserData = request;
            connection.BeginSend(request);
        }
        /// <summary>
        /// disconnected
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        private void OnDisconnected(IConnection connection, Exception ex)
        {
            this._currConnection = null;
            connection.Disconnected -= this.OnDisconnected;
            TaskEx.Delay(new Random().Next(10, 50), this.CreateSocket);
        }
        /// <summary>
        /// fire connected
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        private void FireConnected(string name, IConnection connection)
        {
            this.Connected(name, connection);
        }
        /// <summary>
        /// fire ServerAvailable
        /// </summary>
        private void FireServerAvailable()
        {
            this.ServerAvailable();
        }
        #endregion
    }
}