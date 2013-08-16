using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// stat
    /// </summary>
    public sealed class Stat : IRecord
    {
        #region Public Methods
        /// <summary>
        /// 导致此ZNode创建的zxid（ZooKeeper事务ID）
        /// </summary>
        public long Czxid
        {
            get;
            private set;
        }
        /// <summary>
        /// 最后一次修改此ZNode的zxid
        /// </summary>
        public long Mzxid
        {
            get;
            private set;
        }
        /// <summary>
        /// 从此ZNode创建到现在为止的时间毫秒数
        /// </summary>
        public long Ctime
        {
            get;
            private set;
        }
        /// <summary>
        /// 从此ZNode上次修改到现在为止的时间毫秒数
        /// </summary>
        public long Mtime
        {
            get;
            private set;
        }
        /// <summary>
        /// 此Znode的数据修改次数
        /// </summary>
        public int Version
        {
            get;
            private set;
        }
        /// <summary>
        /// 此ZNode的子节点修改次数
        /// </summary>
        public int Cversion
        {
            get;
            private set;
        }
        /// <summary>
        /// 此ZNode的ACL修改次数
        /// </summary>
        public int Aversion
        {
            get;
            private set;
        }
        /// <summary>
        /// 如果此ZNode是一个瞬时节点，此值表示此ZNode对应的会话ID。如果不是瞬时节点则为0
        /// </summary>
        public long EphemeralOwner
        {
            get;
            private set;
        }
        /// <summary>
        /// 此ZNode中数据的长度
        /// </summary>
        public int DataLength
        {
            get;
            private set;
        }
        /// <summary>
        /// 此ZNode的子节点数量
        /// </summary>
        public int NumChildren
        {
            get;
            private set;
        }
        /// <summary>
        /// pzxid
        /// </summary>
        public long Pzxid
        {
            get;
            private set;
        }
        #endregion

        #region IRecord Members
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
            this.Czxid = formatter.ReadInt64();
            this.Mzxid = formatter.ReadInt64();
            this.Ctime = formatter.ReadInt64();
            this.Mtime = formatter.ReadInt64();
            this.Version = formatter.ReadInt32();
            this.Cversion = formatter.ReadInt32();
            this.Aversion = formatter.ReadInt32();
            this.EphemeralOwner = formatter.ReadInt64();
            this.DataLength = formatter.ReadInt32();
            this.NumChildren = formatter.ReadInt32();
            this.Pzxid = formatter.ReadInt64();
        }
        #endregion
    }
}