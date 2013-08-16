using System;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// session node
    /// </summary>
    public sealed class SessionNode
    {
        #region Private Members
        private readonly IZookClient _zk = null;
        private readonly NodeInfo _nodeInfo = null;
        private bool _isClosed = false;
        private bool _nodeCreating = false;
        #endregion

        #region Constructors
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
            lock (this)
            {
                if (this._isClosed || this._nodeCreating) return;
                this._nodeCreating = true;
            }

            NodeFactory.TryEnsureCreate(this._zk, this._nodeInfo, () =>
            {
                lock (this)
                {
                    this._nodeCreating = false;
                    if (this._isClosed) { this.RemoveNode(); return; }
                }
            });
        }
        /// <summary>
        /// remove node.
        /// </summary>
        private void RemoveNode()
        {
            this._zk.Delete(this._nodeInfo.Path);
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

        #region Public Methods
        /// <summary>
        /// close
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (this._isClosed) return;
                this._isClosed = true;

                this._zk.KeeperStateChanged -= new KeeperStateChangedHandler(this.KeeperStateChanged);

                if (this._nodeCreating) return;
                this.RemoveNode();
            }
        }
        #endregion
    }
}