using System;
using System.IO;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// create request
    /// </summary>
    public sealed class CreateRequest : IRecord
    {
        #region Public Members
        /// <summary>
        /// path
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// path data
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// acl array
        /// </summary>
        public readonly Data.ACL[] Acl;
        /// <summary>
        /// flags
        /// </summary>
        public readonly int Flags;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="acl"></param>
        /// <param name="flags"></param>
        public CreateRequest(string path, byte[] data, Data.ACL[] acl, int flags)
        {
            this.Path = path;
            this.Data = data;
            this.Acl = acl;
            this.Flags = flags;
        }
        #endregion

        #region IRecord Members
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.Path);
            formatter.Write(this.Data);
            formatter.Write(this.Acl);
            formatter.Write(this.Flags);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}