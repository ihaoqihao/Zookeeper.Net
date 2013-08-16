using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// get children v2 response
    /// </summary>
    public sealed class GetChildren2Response : IRecord
    {
        /// <summary>
        /// Children
        /// </summary>
        public string[] Children
        {
            get;
            private set;
        }
        /// <summary>
        /// Stat
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
            this.Children = formatter.ReadStringArray();
            this.Stat = formatter.ReadRecord<Stat>();
        }
    }
}