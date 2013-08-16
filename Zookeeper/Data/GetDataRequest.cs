using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// get data request
    /// </summary>
    public sealed class GetDataRequest : IRecord
    {
        /// <summary>
        /// path
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// true表示侦听
        /// </summary>
        public readonly bool Watch;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watch"></param>
        public GetDataRequest(string path, bool watch)
        {
            this.Path = path;
            this.Watch = watch;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Path);
            formatter.Write(this.Watch);
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