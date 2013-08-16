using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// delete node request
    /// </summary>
    public sealed class DeleteRequest : IRecord
    {
        /// <summary>
        /// path
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// version
        /// </summary>
        public readonly int Version;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="version"></param>
        public DeleteRequest(string path, int version)
        {
            this.Path = path;
            this.Version = version;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Path);
            formatter.Write(this.Version);
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