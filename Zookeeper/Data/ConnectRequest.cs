using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// connect request
    /// </summary>
    public sealed class ConnectRequest : IRecord
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="protocolVersion"></param>
        /// <param name="lastZxidSeen"></param>
        /// <param name="timeOut"></param>
        /// <param name="sessionID"></param>
        /// <param name="passwd"></param>
        public ConnectRequest(int protocolVersion, long lastZxidSeen, int timeOut, long sessionID, byte[] passwd)
        {
            ProtocolVersion = protocolVersion;
            LastZxidSeen = lastZxidSeen;
            SessionTimeOut = timeOut;
            SessionID = sessionID;
            SessionPassword = passwd;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 协议version
        /// </summary>
        public int ProtocolVersion
        {
            get;
            private set;
        }
        /// <summary>
        /// last zxid seen
        /// </summary>
        public long LastZxidSeen
        {
            get;
            private set;
        }
        /// <summary>
        /// timeout
        /// </summary>
        public int SessionTimeOut
        {
            get;
            private set;
        }
        /// <summary>
        /// sessionid
        /// </summary>
        public long SessionID
        {
            get;
            private set;
        }
        /// <summary>
        /// password
        /// </summary>
        public byte[] SessionPassword
        {
            get;
            private set;
        }
        #endregion

        #region IRecord Members
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.ProtocolVersion);
            formatter.Write(this.LastZxidSeen);
            formatter.Write(this.SessionTimeOut);
            formatter.Write(this.SessionID);
            formatter.Write(this.SessionPassword);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}