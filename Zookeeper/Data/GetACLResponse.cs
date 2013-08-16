
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// get ACL response
    /// </summary>
    public sealed class GetACLResponse : IRecord
    {
        /// <summary>
        /// acl array
        /// </summary>
        public ACL[] Acl
        {
            get;
            private set;
        }
        /// <summary>
        /// node stat
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
        }
        /// <summary>
        /// read
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            this.Acl = formatter.ReadRecordList<ACL>();
            this.Stat = formatter.ReadRecord<Stat>();
        }
    }
}