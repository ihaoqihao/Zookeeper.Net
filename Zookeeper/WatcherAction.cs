using System;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// watcher action
    /// </summary>
    public sealed class WatcherAction : IWatcher
    {
        private readonly Action<Data.WatchedEvent> _callback = null;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException">callback is null.</exception>
        public WatcherAction(Action<Data.WatchedEvent> callback)
        {
            if (callback == null) throw new ArgumentNullException("action");
            this._callback = callback;
        }

        /// <summary>
        /// invoke event.
        /// </summary>
        /// <param name="zevent"></param>
        public void Invoke(Data.WatchedEvent zevent)
        {
            this._callback(zevent);
        }
    }
}