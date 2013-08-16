
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// set ACL request
    /// </summary>
    public sealed class SetACLRequest : IRecord
    {
        /// <summary>
        /// the path of the node
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// acl to set
        /// </summary>
        public readonly ACL[] Acl;
        /// <summary>
        /// version
        /// </summary>
        public readonly int Version;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="acl"></param>
        /// <param name="version"></param>
        public SetACLRequest(string path, ACL[] acl, int version)
        {
            this.Path = path;
            this.Acl = acl;
            this.Version = version;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Path);
            formatter.Write(this.Acl);
            formatter.Write(this.Version);
        }
        /// <summary>
        /// read
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            throw new System.NotImplementedException();
        }
    }
}