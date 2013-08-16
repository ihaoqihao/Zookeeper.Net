
namespace Sodao.Zookeeper
{
    /// <summary>
    /// node info
    /// </summary>
    public sealed class NodeInfo
    {
        /// <summary>
        /// get node path
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// get node data
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// get node acl
        /// </summary>
        public readonly Data.ACL[] ACL;
        /// <summary>
        /// get node create mode
        /// </summary>
        public readonly Data.CreateMode CreateMode;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acl"></param>
        /// <param name="createMode"></param>
        public NodeInfo(string path, byte[] data, Data.ACL[] acl, Data.CreateMode createMode)
        {
            this.Path = path;
            this.Data = data;
            this.ACL = acl;
            this.CreateMode = createMode;
        }
    }
}