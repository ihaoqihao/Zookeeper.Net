using Sodao.FastSocket.Client;
using Sodao.FastSocket.Client.Protocol;
using Sodao.FastSocket.SocketBase;
using Sodao.FastSocket.SocketBase.Utils;
using System;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper protocol
    /// </summary>
    public sealed class ZookProtocol : IProtocol<ZookResponse>
    {
        /// <summary>
        /// defautl seqId
        /// </summary>
        public int DefaultSyncSeqID
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// true
        /// </summary>
        public bool IsAsync
        {
            get { return true; }
        }
        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        /// <exception cref="BadProtocolException"></exception>
        public ZookResponse Parse(IConnection connection, ArraySegment<byte> buffer, out int readlength)
        {
            if (buffer.Count < 4)
            {
                readlength = 0;
                return null;
            }

            var payload = buffer.Array;

            //获取message length
            var messageLength = NetworkBitConverter.ToInt32(payload, buffer.Offset);
            readlength = messageLength + 4;
            if (buffer.Count < readlength)
            {
                readlength = 0;
                return null;
            }

            //首次connect to zk response
            var connectRequest = connection.UserData as Request<ZookResponse>;
            if (connectRequest != null)
            {
                connection.UserData = null;
                var connectResponseBuffer = new byte[messageLength];
                Buffer.BlockCopy(payload, buffer.Offset + 4, connectResponseBuffer, 0, messageLength);
                return new ZookResponse(connectRequest.SeqID, connectResponseBuffer);
            }

            var xid = NetworkBitConverter.ToInt32(payload, buffer.Offset + 4);
            var zxid = NetworkBitConverter.ToInt64(payload, buffer.Offset + 8);
            var err = NetworkBitConverter.ToInt32(payload, buffer.Offset + 16);

            var bodylength = messageLength - 16;
            if (bodylength < 0) throw new BadProtocolException("illegality message length.");

            byte[] bodyBytes = null;
            if (bodylength > 0)
            {
                bodyBytes = new byte[bodylength];
                Buffer.BlockCopy(payload, buffer.Offset + 20, bodyBytes, 0, bodyBytes.Length);
            }
            return new ZookResponse(xid, zxid, (Data.ZoookError)err, bodyBytes);
        }
    }
}