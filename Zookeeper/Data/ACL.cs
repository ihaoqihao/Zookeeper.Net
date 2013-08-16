
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// acl
    /// </summary>
    public sealed class ACL : IRecord
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        public ACL()
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="perms"></param>
        /// <param name="id"></param>
        public ACL(Perms perms, ZookID id)
        {
            this.Perms = perms;
            this.ID = id;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 权限
        /// </summary>
        public Perms Perms
        {
            get;
            set;
        }
        /// <summary>
        /// id
        /// </summary>
        public ZookID ID
        {
            get;
            set;
        }
        #endregion

        #region IRecord Members
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write((int)this.Perms);
            formatter.Write(this.ID);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            this.Perms = (Perms)formatter.ReadInt32();
            this.ID = formatter.ReadRecord<ZookID>();
        }
        #endregion
    }
}