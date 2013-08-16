
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// zookeeper id
    /// </summary>
    public sealed class ZookID : IRecord
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        public ZookID()
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="id"></param>
        public ZookID(string scheme, string id)
        {
            this.Scheme = scheme;
            this.ID = id;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Scheme
        /// </summary>
        public string Scheme
        {
            get;
            set;
        }
        /// <summary>
        /// ID
        /// </summary>
        public string ID
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
            formatter.Write(this.Scheme);
            formatter.Write(this.ID);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            this.Scheme = formatter.ReadString();
            this.ID = formatter.ReadString();
        }
        #endregion
    }
}