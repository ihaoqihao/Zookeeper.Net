
namespace Sodao.Zookeeper
{
    /// <summary>
    /// a zookeeper watcher interface
    /// </summary>
    public interface IWatcher
    {
        /// <summary>
        /// invoke event.
        /// </summary>
        /// <param name="zevent"></param>
        void Invoke(Data.WatchedEvent zevent);
    }
}