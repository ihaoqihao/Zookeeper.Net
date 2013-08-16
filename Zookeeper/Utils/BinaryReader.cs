using System;
using System.IO;

namespace Sodao.Zookeeper.Utils
{
    /// <summary>
    /// binary reader
    /// </summary>
    public sealed class BinaryReader : IDisposable
    {
        #region Private Members
        private Stream _stream = null;
        private byte[] _tempBuffer = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="ArgumentNullException">stream is null</exception>
        public BinaryReader(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            this._stream = stream;
            this._tempBuffer = new byte[8];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// read byte
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            this._stream.Read(this._tempBuffer, 0, 1);
            return this._tempBuffer[0];
        }
        /// <summary>
        /// read buffer
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBuffer(int count)
        {
            var payload = new byte[count];
            this._stream.Read(payload, 0, count);
            return payload;
        }
        /// <summary>
        /// read bool
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            this._stream.Read(this._tempBuffer, 0, 1);
            return this._tempBuffer[0] == 1;
        }
        /// <summary>
        /// read int16
        /// </summary>
        /// <returns></returns>
        public short ReadInt16()
        {
            this._stream.Read(this._tempBuffer, 0, 2);
            return (short)(((this._tempBuffer[0] & 0xff) << 8) | ((this._tempBuffer[1] & 0xff)));
        }
        /// <summary>
        /// read int32
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            this._stream.Read(this._tempBuffer, 0, 4);
            return (int)(((this._tempBuffer[0] & 0xff) << 24) | ((this._tempBuffer[1] & 0xff) << 16) | ((this._tempBuffer[2] & 0xff) << 8) | ((this._tempBuffer[3] & 0xff)));
        }
        /// <summary>
        /// read int64
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            this._stream.Read(this._tempBuffer, 0, 8);
            return (long)(((long)(this._tempBuffer[0] & 0xff) << 56) |
                ((long)(this._tempBuffer[1] & 0xff) << 48) |
                ((long)(this._tempBuffer[2] & 0xff) << 40) |
                ((long)(this._tempBuffer[3] & 0xff) << 32) |
                ((long)(this._tempBuffer[4] & 0xff) << 24) |
                ((long)(this._tempBuffer[5] & 0xff) << 16) |
                ((long)(this._tempBuffer[6] & 0xff) << 8) |
                ((long)(this._tempBuffer[7] & 0xff)));
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 获取 <see cref="BinaryReader"/> 的基础流。
        /// </summary>
        public Stream BaseStream
        {
            get { return this._stream; }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this._stream.Dispose();
            this._stream = null;
        }
        #endregion
    }
}