using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// get ACL request.
    /// </summary>
    public sealed class GetACLRequest : IRecord
    {
        /// <summary>
        /// the path of the node.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        public GetACLRequest(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Path);
        }
        /// <summary>
        /// read
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
    }
}