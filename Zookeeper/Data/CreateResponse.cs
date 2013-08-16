using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// create response
    /// </summary>
    public sealed class CreateResponse : IRecord
    {
        /// <summary>
        /// path
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// read
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            this.Path = formatter.ReadString();
        }
    }
}