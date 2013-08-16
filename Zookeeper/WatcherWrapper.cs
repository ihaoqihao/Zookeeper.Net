using System;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// watcher wrapper
    /// </summary>
    public sealed class WatcherWrapper : IWatcher
    {
        private readonly Action<Data.WatchedEvent> _callback = null;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="callback"></param>
        public WatcherWrapper(Action<Data.WatchedEvent> callback)
        {
            if (callback == null) throw new ArgumentNullException("action");
            this._callback = callback;
        }

        /// <summary>
        /// process
        /// </summary>
        /// <param name="zevent"></param>
        public void Process(Data.WatchedEvent zevent)
        {
            this._callback(zevent);
        }
    }
}