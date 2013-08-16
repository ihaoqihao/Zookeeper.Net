
namespace Sodao.Zookeeper
{
    /// <summary>
    /// a zookeeper watcher interface
    /// </summary>
    public interface IWatcher
    {
        /// <summary>
        /// 处理相关事件
        /// </summary>
        /// <param name="zevent"></param>
        void Process(Data.WatchedEvent zevent);
    }
}