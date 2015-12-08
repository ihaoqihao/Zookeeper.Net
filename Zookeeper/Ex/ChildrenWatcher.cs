using Sodao.FastSocket.SocketBase.Utils;
using System;
using System.Threading;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zk node children watcher
    /// </summary>
    public sealed class ChildrenWatcher : IDisposable
    {
        #region Private Members
        private readonly IZookClient _zk = null;
        private readonly string _path;
        private readonly Action<string[]> _callback;
        private readonly WatcherAction _watcher = null;

        private int _isdisposed = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// free
        /// </summary>
        ~ChildrenWatcher()
        {
            this.Dispose();
        }
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

            this._watcher = new WatcherAction(_ => this.ListChildren());
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
            if (currentState == Data.KeeperState.SyncConnected)
                this.ListChildren();
        }
        /// <summary>
        /// list children
        /// </summary>
        private void ListChildren()
        {
            if (Thread.VolatileRead(ref this._isdisposed) == 1) return;

            this._zk.GetChildren(this._path, this._watcher).ContinueWith(c =>
            {
                if (Thread.VolatileRead(ref this._isdisposed) == 1) return;
                if (c.IsFaulted)
                {
                    TaskEx.Delay(new Random().Next(500, 1500)).ContinueWith(_ => this.ListChildren());
                    return;
                }
                this._callback(c.Result);
            });
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this._isdisposed, 1, 0) == 1) return;

            this._zk.KeeperStateChanged -= this.KeeperStateChanged;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}