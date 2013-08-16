using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// set data request
    /// </summary>
    public sealed class SetDataRequest : IRecord
    {
        /// <summary>
        /// the path of the node
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// data to set
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// version
        /// </summary>
        public readonly int Version;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="version"></param>
        public SetDataRequest(string path, byte[] data, int version)
        {
            this.Path = path;
            this.Data = data;
            this.Version = version;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Path);
            formatter.Write(this.Data);
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