
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// zookeeper state枚举
    /// </summary>
    public enum KeeperState : int
    {
        /// <summary>
        /// unknow
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 已断开
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// no同步连接
        /// </summary>
        NoSyncConnected = 1,
        /// <summary>
        /// 同步连接
        /// </summary>
        SyncConnected = 3,
        /// <summary>
        /// 过期
        /// </summary>
        Expired = -112
    }
}