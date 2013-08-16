
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// event type枚举
    /// </summary>
    public enum EventType : int
    {
        /// <summary>
        /// none
        /// </summary>
        None = -1,
        /// <summary>
        /// 节点已创建
        /// </summary>
        NodeCreated = 1,
        /// <summary>
        /// 节点已被删除
        /// </summary>
        NodeDeleted = 2,
        /// <summary>
        /// 节点数据已更改
        /// </summary>
        NodeDataChanged = 3,
        /// <summary>
        /// 子节点已更改
        /// </summary>
        NodeChildrenChanged = 4
    }
}