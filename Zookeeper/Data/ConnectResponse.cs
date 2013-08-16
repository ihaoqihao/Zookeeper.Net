using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// connect response
    /// </summary>
    public sealed class ConnectResponse : IRecord
    {
        #region Public Properties
        /// <summary>
        /// 协议version
        /// </summary>
        public int ProtocolVersion
        {
            get;
            private set;
        }
        /// <summary>
        /// timeout
        /// </summary>
        public int SessionTimeOut
        {
            get;
            private set;
        }
        /// <summary>
        /// sessionid
        /// </summary>
        public long SessionID
        {
            get;
            private set;
        }
        /// <summary>
        /// password
        /// </summary>
        public byte[] SessionPassword
        {
            get;
            private set;
        }
        #endregion

        #region IRecord Members
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            this.ProtocolVersion = formatter.ReadInt32();
            this.SessionTimeOut = formatter.ReadInt32();
            this.SessionID = formatter.ReadInt64();
            this.SessionPassword = formatter.ReadBuffer();
        }
        #endregion
    }
}