using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sodao.FastSocket.Client;
using Sodao.FastSocket.SocketBase;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper client
    /// </summary>
    public sealed class ZookClient : PooledSocketClient<ZookResponse>, IZookClient
    {
        #region Private Members
        private ZookServerPool _zkServerPool = null;

        private volatile Data.KeeperState _currentState = Data.KeeperState.Disconnected;
        private readonly string _chrootPath = null;
        internal readonly TimeSpan _sessionTimeout;
        internal long _lastZxid = 0;//最近一次的zk事务ID
        private readonly ZookWatcherManager _watcherManager = null;
        private readonly List<Data.AuthRequest> _authInfolist = new List<Data.AuthRequest>();

        private Timer _timer = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        ~ZookClient()
        {
            this.CloseSession();
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="chrootPath"></param>
        /// <param name="connectionString">127.0.0.1:2181,10.0.51.14:2181</param>
        /// <param name="sessionTimeout"></param>
        /// <param name="defaultWatcher"></param>
        /// <exception cref="ArgumentNullException">connectionString is null or empty.</exception>
        public ZookClient(string chrootPath, string connectionString, TimeSpan sessionTimeout, IWatcher defaultWatcher = null)
            : base(new ZookProtocol(), 8192, 8192, 3000, 3000)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");
            if (!string.IsNullOrEmpty(chrootPath)) Utils.PathUtils.ValidatePath(chrootPath);

            this._chrootPath = chrootPath;
            this._sessionTimeout = sessionTimeout;
            this._watcherManager = new ZookWatcherManager(defaultWatcher);
            this._zkServerPool.SetOptions(this, connectionString);

            this.StartLoopPing();
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// InitServerPool
        /// </summary>
        /// <returns></returns>
        protected override IServerPool InitServerPool()
        {
            return this._zkServerPool = new ZookServerPool();
        }
        /// <summary>
        /// on server available
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        protected override void OnServerPoolServerAvailable(string name, IConnection connection)
        {
            base.OnServerPoolServerAvailable(name, connection);
            this.ResetAuthInfo(connection);
            this.ResetWatches(connection);
        }
        /// <summary>
        /// OnSendFailed
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="request"></param>
        protected override void OnSendFailed(IConnection connection, Request<ZookResponse> request)
        {
            if (request.Tag == null) base.OnSendFailed(connection, request);
        }
        /// <summary>
        /// OnDisconnected
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);
            this.SetKeeperState(Data.KeeperState.Disconnected);
        }
        /// <summary>
        /// 处理未知response
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="response"></param>
        protected override void HandleUnknowResponse(IConnection connection, ZookResponse response)
        {
            if (response.HasError()) { connection.BeginDisconnect(response.Error()); return; }
            if (response.XID != -1) return;

            Data.WatcherEvent wevent = null;
            try { wevent = Utils.Marshaller.Deserialize<Data.WatcherEvent>(response.Payload); }
            catch (Exception ex) { Sodao.FastSocket.SocketBase.Log.Trace.Error(ex.Message, ex); return; }

            var e = new Data.WatchedEvent(wevent.Type, wevent.State, Utils.PathUtils.RemoveChroot(this._chrootPath, wevent.Path));
            this._watcherManager.Invoke(e);
        }
        #endregion

        #region IZookClient Members
        /// <summary>
        /// keeper state changed event
        /// </summary>
        public event KeeperStateChangedHandler KeeperStateChanged;

        /// <summary>
        /// get current keeperState
        /// </summary>
        public Data.KeeperState CurrentKeeperState
        {
            get { return this._currentState; }
        }
        /// <summary>
        /// Add the specified scheme:auth information to this connection.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="auth"></param>
        public void AddAuthInfo(string scheme, byte[] auth)
        {
            lock (this._authInfolist) this._authInfolist.Add(new Data.AuthRequest(0, scheme, auth));
            this.SendPacket(new Packet(Utils.Marshaller.Serialize(-4, Data.OpCode.Auth, new Data.AuthRequest(0, scheme, auth), true)));
        }
        /// <summary>
        /// Specify the default watcher for the connection (overrides the one
        /// specified during construction).
        /// </summary>
        /// <param name="watcher"></param>
        public void Register(IWatcher watcher)
        {
            this._watcherManager.DefaultWatcher = watcher;
        }
        /// <summary>
        /// Create a node with the given path. The node data will be the given data,
        /// and node acl will be the given acl.
        /// </summary>
        /// <param name="path">The path for the node.</param>
        /// <param name="data">The data for the node.</param>
        /// <param name="acl">The acl for the node.</param>
        /// <param name="createMode">specifying whether the node to be created is ephemeral and/or sequential.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">createMode is null</exception>
        public Task<string> Create(string path, byte[] data, Data.ACL[] acl, Data.CreateMode createMode)
        {
            if (createMode == null) throw new ArgumentNullException("createMode");

            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath, createMode.Sequential);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<string>(base.NextRequestSeqID(), Data.OpCode.Create,
                new Data.CreateRequest(serverPath, data, acl, createMode.Flag),
                (src, response) =>
                {
                    if (response.HasError()) { src.TrySetException(response.Error(clientPath)); return; }

                    Data.CreateResponse result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.CreateResponse>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(Utils.PathUtils.RemoveChroot(this._chrootPath, result.Path));
                });
        }
        /// <summary>
        /// Delete the node with the given path. The call will succeed if such a node
        /// exists, and the given version matches the node's version (if the given
        /// version is -1, it matches any node's versions).
        ///
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if the nodes does not exist.
        ///
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        ///
        /// A KeeperException with error code KeeperException.NotEmpty will be thrown
        /// if the node has children.
        /// 
        /// This operation, if successful, will trigger all the watches on the node
        /// of the given path left by exists API calls, and the watches on the parent
        /// node left by getChildren API calls.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="version">The version.</param>
        public Task Delete(string path, int version)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<bool>(base.NextRequestSeqID(), Data.OpCode.Delete, new Data.DeleteRequest(serverPath, version),
                (src, response) =>
                {
                    if (response.HasError()) { src.TrySetException(response.Error(clientPath)); return; }
                    src.TrySetResult(true);
                });
        }
        /// <summary>
        /// Return the stat of the node of the given path. Return null if no such a
        /// node exists.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch will be
        /// triggered by a successful operation that creates/delete the node or sets
        /// the data on the node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="watch">whether need to watch this node</param>
        /// <returns>the stat of the node of the given path; return null if no such a node exists.</returns>
        ///<exception cref="ArgumentNullException">DefaultWatcher is null</exception>
        public Task<Data.Stat> Exists(string path, bool watch)
        {
            if (watch && this._watcherManager.DefaultWatcher == null) throw new ArgumentNullException("DefaultWatcher");
            return this.Exists(path, watch ? this._watcherManager.DefaultWatcher : null);
        }
        /// <summary>
        /// Return the stat of the node of the given path. Return null if no such a
        /// node exists.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch will be
        /// triggered by a successful operation that creates/delete the node or sets
        /// the data on the node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="watcher">The watcher.</param>
        /// <returns>the stat of the node of the given path; return null if no such a node exists.</returns>
        public Task<Data.Stat> Exists(string path, IWatcher watcher)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<Data.Stat>(base.NextRequestSeqID(), Data.OpCode.Exists,
                new Data.ExistsRequest(serverPath, watcher != null),
                (src, response) =>
                {
                    if (response.HasError() && response.ErrorCode != Data.ZoookError.NONODE)
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (watcher != null)//register watcher
                    {
                        if (response.HasError()) this._watcherManager.RegisterExistWatcher(watcher, clientPath);
                        else this._watcherManager.RegisterDataWatcher(watcher, clientPath);
                    }

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.Stat result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.Stat>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result);
                });
        }
        /// <summary>
        /// Return the data and the stat of the node of the given path.
        /// </summary>
        /// <param name="path">the given path</param>
        /// <param name="watch">whether need to watch this node</param>
        /// <returns></returns>
        public Task<Data.GetDataResponse> GetData(string path, bool watch)
        {
            if (watch && this._watcherManager.DefaultWatcher == null) throw new ArgumentNullException("DefaultWatcher");
            return this.GetData(path, watch ? this._watcherManager.DefaultWatcher : null);
        }
        /// <summary>
        /// Return the data and the stat of the node of the given path.
        /// </summary>
        /// <param name="path">the given path</param>
        /// <param name="watcher">explicit watcher</param>
        /// <returns></returns>
        public Task<Data.GetDataResponse> GetData(string path, IWatcher watcher)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<Data.GetDataResponse>(base.NextRequestSeqID(), Data.OpCode.GetData,
                new Data.GetDataRequest(serverPath, watcher != null),
                (src, response) =>
                {
                    if (response.HasError())
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (watcher != null) this._watcherManager.RegisterDataWatcher(watcher, clientPath);//register watcher

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.GetDataResponse result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.GetDataResponse>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result);
                });
        }
        /// <summary>
        /// Set the data for the node of the given path if such a node exists and the
        /// given version matches the version of the node (if the given version is
        /// -1, it matches any node's versions). Return the stat of the node.
        /// 
        /// This operation, if successful, will trigger all the watches on the node
        /// of the given path left by getData calls.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// 
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        ///
        /// The maximum allowable size of the data array is 1 MB (1,048,576 bytes).
        /// Arrays larger than this will cause a KeeperExecption to be thrown.
        /// </summary>
        /// <param name="path">the path of the node</param>
        /// <param name="data">the data to set</param>
        /// <param name="version">the expected matching version</param>
        /// <returns>the state of the node</returns>
        public Task<Data.Stat> SetData(string path, byte[] data, int version)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<Data.Stat>(base.NextRequestSeqID(), Data.OpCode.SetData,
                new Data.SetDataRequest(serverPath, data, version),
                (src, response) =>
                {
                    if (response.HasError())
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.Stat result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.Stat>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result);
                });
        }
        /// <summary>
        /// Return the ACL and stat of the node of the given path.
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path">the given path for the node</param>
        /// <returns></returns>
        public Task<Data.GetACLResponse> GetACL(string path)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<Data.GetACLResponse>(base.NextRequestSeqID(), Data.OpCode.GetACL,
                new Data.GetACLRequest(serverPath),
                (src, response) =>
                {
                    if (response.HasError())
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.GetACLResponse result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.GetACLResponse>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result);
                });
        }
        /// <summary>
        /// Set the ACL for the node of the given path if such a node exists and the
        /// given version matches the version of the node. Return the stat of the
        /// node.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// 
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="acl"></param>
        /// <param name="version"></param>
        /// <returns>the stat of the node.</returns>
        public Task<Data.Stat> SetACL(string path, Data.ACL[] acl, int version)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<Data.Stat>(base.NextRequestSeqID(), Data.OpCode.SetACL,
                new Data.SetACLRequest(serverPath, acl, version),
                (src, response) =>
                {
                    if (response.HasError())
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.Stat result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.Stat>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result);
                });
        }
        /// <summary>
        /// Return the list of the children of the node of the given path.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watch"></param>
        /// <returns>an unordered array of children of the node with the given path</returns>
        /// <exception cref="ArgumentNullException">DefaultWatcher is null</exception>
        public Task<string[]> GetChildren(string path, bool watch)
        {
            if (watch && this._watcherManager.DefaultWatcher == null) throw new ArgumentNullException("DefaultWatcher");
            return this.GetChildren(path, watch ? this._watcherManager.DefaultWatcher : null);
        }
        /// <summary>
        /// Return the list of the children of the node of the given path.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watcher"></param>
        /// <returns>an unordered array of children of the node with the given path</returns>
        public Task<string[]> GetChildren(string path, IWatcher watcher)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<string[]>(base.NextRequestSeqID(), Data.OpCode.GetChildren,
                new Data.GetChildrenRequest(serverPath, watcher != null),
                (src, response) =>
                {
                    if (response.HasError())
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (watcher != null) this._watcherManager.RegisterChildWatcher(watcher, clientPath);//register watcher

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.GetChildrenResponse result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.GetChildrenResponse>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result.Children);
                });
        }
        /// <summary>
        /// For the given znode path return the stat and children list.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">DefaultWatcher is null</exception>
        public Task<Data.GetChildren2Response> GetChildren2(string path, bool watch)
        {
            if (watch && this._watcherManager.DefaultWatcher == null) throw new ArgumentNullException("DefaultWatcher");
            return this.GetChildren2(path, watch ? this._watcherManager.DefaultWatcher : null);
        }
        /// <summary>
        /// For the given znode path return the stat and children list.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watcher"></param>
        /// <returns></returns>
        public Task<Data.GetChildren2Response> GetChildren2(string path, IWatcher watcher)
        {
            var clientPath = path;
            Utils.PathUtils.ValidatePath(clientPath);
            var serverPath = Utils.PathUtils.PrependChroot(this._chrootPath, clientPath);

            return this.ExecuteAsync<Data.GetChildren2Response>(base.NextRequestSeqID(), Data.OpCode.GetChildren2,
                new Data.GetChildrenRequest(serverPath, watcher != null),
                (src, response) =>
                {
                    if (response.HasError())
                    {
                        src.TrySetException(response.Error(clientPath)); return;
                    }

                    if (watcher != null) this._watcherManager.RegisterChildWatcher(watcher, clientPath);//register watcher

                    if (response.Payload == null || response.Payload.Length == 0)
                    {
                        src.TrySetResult(null); return;
                    }

                    Data.GetChildren2Response result = null;
                    try { result = Utils.Marshaller.Deserialize<Data.GetChildren2Response>(response.Payload); }
                    catch (Exception ex) { src.TrySetException(ex); return; }

                    src.TrySetResult(result);
                });
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// reset watches
        /// </summary>
        /// <param name="connection"></param>
        private void ResetWatches(IConnection connection)
        {
            var dataKeys = this._watcherManager.GetDataWatchKeys();
            var existKeys = this._watcherManager.GetExistWatchKeys();
            var childKeys = this._watcherManager.GetChildWatchKeys();

            if (dataKeys.Length > 0 || existKeys.Length > 0 || childKeys.Length > 0)
            {
                this.SendPacket(connection, new Packet(Utils.Marshaller.Serialize(-8, Data.OpCode.SetWatches,
                    new Data.SetWatchesRequest(this._lastZxid,
                        dataKeys.Select(c => Utils.PathUtils.PrependChroot(this._chrootPath, c)).ToArray(),
                        existKeys.Select(c => Utils.PathUtils.PrependChroot(this._chrootPath, c)).ToArray(),
                        childKeys.Select(c => Utils.PathUtils.PrependChroot(this._chrootPath, c)).ToArray()), true)));
            }
        }
        /// <summary>
        /// reset auth info
        /// </summary>
        /// <param name="connection"></param>
        private void ResetAuthInfo(IConnection connection)
        {
            Data.AuthRequest[] arrAuthRequests = null;
            lock (this._authInfolist) { arrAuthRequests = this._authInfolist.ToArray(); }
            if (arrAuthRequests.Length == 0) return;

            foreach (var child in arrAuthRequests)
                this.SendPacket(connection, new Packet(Utils.Marshaller.Serialize(-4, Data.OpCode.Auth, child, true)));
        }
        /// <summary>
        /// send packet
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private bool SendPacket(Packet packet)
        {
            return this.SendPacket(this._zkServerPool.Acquire(), packet);
        }
        /// <summary>
        /// send packet
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        private bool SendPacket(IConnection connection, Packet packet)
        {
            if (connection == null) return false;
            connection.BeginSend(packet); return true;
        }
        /// <summary>
        /// execute async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xid"></param>
        /// <param name="code"></param>
        /// <param name="record"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">callback is null.</exception>
        private Task<T> ExecuteAsync<T>(int xid, Data.OpCode code, Data.IRecord record,
            Action<TaskCompletionSource<T>, ZookResponse> callback, object asyncState = null)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            var source = new TaskCompletionSource<T>(asyncState);
            this.Send(new Request<ZookResponse>(xid, code.ToString(), Utils.Marshaller.Serialize(xid, code, record, true),
                ex => source.TrySetException(ex),
                response =>
                {
                    if (response.ZXID > 0) this._lastZxid = response.ZXID;
                    callback(source, response);
                }));

            return source.Task;
        }
        /// <summary>
        /// start loop ping
        /// </summary>
        private void StartLoopPing()
        {
            this._timer = new Timer(_ =>
                this.SendPacket(new Packet(Utils.Marshaller.Serialize(-2, Data.OpCode.Ping, null, true))),
                null, 0, 3000);
        }
        /// <summary>
        /// close session
        /// </summary>
        private void CloseSession()
        {
            this.SendPacket(new Packet(Utils.Marshaller.Serialize(base.NextRequestSeqID(), Data.OpCode.CloseSession, null, true)));
        }
        /// <summary>
        /// set keeper state
        /// </summary>
        /// <param name="state"></param>
        internal void SetKeeperState(Data.KeeperState state)
        {
            this._currentState = state;
            if (this.KeeperStateChanged != null) this.KeeperStateChanged(state);
        }
        #endregion
    }
}