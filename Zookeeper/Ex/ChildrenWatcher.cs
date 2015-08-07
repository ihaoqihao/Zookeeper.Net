using System;
using Sodao.FastSocket.SocketBase.Utils;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zk node children watcher
    /// </summary>
    public sealed class ChildrenWatcher
    {
        #region Private Members
        private readonly IZookClient _zk = null;
        private readonly string _path;
        private readonly Action<string[]> _callback;
        private readonly WatcherWrapper _wrapper = null;

        private volatile bool _isStop = false;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException">zk is null.</exception>
        /// <exception cref="ArgumentNullException">callback is null.</exception>
        public ChildrenWatcher(IZookClient zk, string path, Action<string[]> callback)
        {
            if (zk == null) throw new ArgumentNullException("zk");
            if (callback == null) throw new ArgumentNullException("callback");

            this._zk = zk;
            this._path = path;
            this._callback = callback;

            this._wrapper = new WatcherWrapper(_ => { if (!this._isStop) this.ListChildren(); });
            this._zk.KeeperStateChanged += this.KeeperStateChanged;

            this.ListChildren();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// KeeperStateChanged
        /// </summary>
        /// <param name="currentState"></param>
        private void KeeperStateChanged(Data.KeeperState currentState)
        {
            if (!this._isStop && currentState == Data.KeeperState.SyncConnected) this.ListChildren();
        }
        /// <summary>
        /// list children
        /// </summary>
        private void ListChildren()
        {
            this._zk.GetChildren(this._path, this._wrapper).ContinueWith(c =>
            {
                if (c.IsFaulted)
                {
                    TaskEx.Delay(new Random().Next(500, 1500)).ContinueWith(_ =>
                        this.ListChildren());
                    return;
                }
                this._callback(c.Result);
            });
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// stop
        /// </summary>
        public void Stop()
        {
            if (this._isStop) return;
            this._isStop = true;
            this._zk.KeeperStateChanged -= this.KeeperStateChanged;
        }
        #endregion
    }
}