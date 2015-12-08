using Sodao.FastSocket.Client.Messaging;
using System;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper response
    /// </summary>
    public class ZookResponse : IMessage
    {
        #region Public Members
        /// <summary>
        /// xid
        /// </summary>
        public readonly int XId;
        /// <summary>
        /// payload
        /// </summary>
        public readonly byte[] Payload;
        /// <summary>
        /// zxid，事务ID
        /// </summary>
        public readonly long ZXId;
        /// <summary>
        /// 错误代码
        /// </summary>
        public readonly Data.ZoookError ErrorCode;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="xid"></param>
        /// <param name="payload"></param>
        public ZookResponse(int xid, byte[] payload)
        {
            this.XId = xid;
            this.Payload = payload;
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="xid"></param>
        /// <param name="zxid"></param>
        /// <param name="errorCode"></param>
        /// <param name="payload"></param>
        public ZookResponse(int xid, long zxid, Data.ZoookError errorCode, byte[] payload)
        {
            this.XId = xid;
            this.ZXId = zxid;
            this.ErrorCode = errorCode;
            this.Payload = payload;
        }
        #endregion

        #region IMessage Members
        /// <summary>
        /// get seqId
        /// </summary>
        public int SeqID
        {
            get { return this.XId; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// true表示当前response有exception
        /// </summary>
        /// <returns></returns>
        public bool HasError()
        {
            return this.ErrorCode != Data.ZoookError.OK;
        }
        /// <summary>
        /// return exception by errorCode
        /// </summary>
        /// <returns></returns>
        public Exception Error()
        {
            return new KeeperException(this.ErrorCode);
        }
        /// <summary>
        /// return exception by errorCode and path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Exception Error(string path)
        {
            return new KeeperException(path, this.ErrorCode);
        }
        #endregion
    }
}