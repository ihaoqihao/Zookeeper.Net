using Sodao.FastSocket.SocketBase.Utils;
using System;
using System.Threading;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// session node
    /// </summary>
    public sealed class SessionNode : IDisposable
    {
        #region Private Members
        private readonly IZookClient _zk = null;
        private readonly NodeInfo _nodeInfo = null;

        private int _isdisposed = 0;
        private int _isCreating = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// free
        /// </summary>
        ~SessionNode()
        {
            this.Dispose();
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acl"></param>
        /// <exception cref="ArgumentNullException">zk is null.</exception>
        public SessionNode(IZookClient zk, string path, byte[] data, Data.ACL[] acl)
        {
            if (zk == null) throw new ArgumentNullException("zk");

            this._zk = zk;
            this._zk.KeeperStateChanged += new KeeperStateChangedHandler(this.KeeperStateChanged);

            this._nodeInfo = new NodeInfo(path, data, acl, Data.CreateModes.Ephemeral);
            this.CreateNode();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// create node
        /// </summary>
        private void CreateNode()
        {
            if (Thread.VolatileRead(ref this._isdisposed) == 1) return;
            if (Interlocked.CompareExchange(ref this._isCreating, 1, 0) == 1) return;

            NodeCreator.TryCreate(this._zk, this._nodeInfo).ContinueWith(c =>
            {
                Thread.VolatileWrite(ref this._isCreating, 0);
                if (Thread.VolatileRead(ref this._isdisposed) == 1)
                {
                    this.RemoveNode();
                    return;
                }

                if (!c.IsFaulted) return;
                TaskEx.Delay(new Random().Next(100, 1500)).ContinueWith(_ => this.CreateNode());
            });
        }
        /// <summary>
        /// remove node
        /// </summary>
        /// <param name="retry"></param>
        private void RemoveNode(int retry = 0)
        {
            if (retry > 10) return;
            this._zk.Delete(this._nodeInfo.Path).ContinueWith(c =>
            {
                if (!c.IsFaulted) return;
                var zkEx = c.Exception.InnerException as KeeperException;
                if (zkEx != null && (
                    zkEx.Error == Data.ZoookError.NONODE ||
                    zkEx.Error == Data.ZoookError.BADVERSION ||
                    zkEx.Error == Data.ZoookError.NOTEMPTY)) return;

                TaskEx.Delay(new Random().Next(10, 500)).ContinueWith(_ => this.RemoveNode(retry + 1));
            });
        }
        /// <summary>
        /// KeeperStateChanged
        /// </summary>
        /// <param name="currentState"></param>
        private void KeeperStateChanged(Data.KeeperState currentState)
        {
            if (currentState == Data.KeeperState.Expired) this.CreateNode();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this._isdisposed, 1, 0) == 1) return;

            this._zk.KeeperStateChanged -= new KeeperStateChangedHandler(this.KeeperStateChanged);
            this.RemoveNode();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}