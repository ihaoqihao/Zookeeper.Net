
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// watched event
    /// </summary>
    public sealed class WatchedEvent
    {
        /// <summary>
        /// type
        /// </summary>
        public readonly EventType Type;
        /// <summary>
        /// state
        /// </summary>
        public readonly KeeperState State;
        /// <summary>
        /// path
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="type"></param>
        /// <param name="state"></param>
        /// <param name="path"></param>
        public WatchedEvent(EventType type, KeeperState state, string path)
        {
            this.Type = type;
            this.State = state;
            this.Path = path;
        }
    }
}