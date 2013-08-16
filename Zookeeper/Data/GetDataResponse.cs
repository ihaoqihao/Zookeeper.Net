using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// get data response
    /// </summary>
    public sealed class GetDataResponse : IRecord
    {
        /// <summary>
        /// get node data
        /// </summary>
        public byte[] Data
        {
            get;
            private set;
        }
        /// <summary>
        /// get node stat
        /// </summary>
        public Stat Stat
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
            this.Data = formatter.ReadBuffer();
            this.Stat = formatter.ReadRecord<Stat>();
        }
    }
}