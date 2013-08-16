using System;
using System.IO;

namespace Sodao.Zookeeper.Utils
{
    /// <summary>
    /// binary writer
    /// </summary>
    public sealed class BinaryWriter : IDisposable
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
        public BinaryWriter(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            this._stream = stream;
            this._tempBuffer = new byte[8];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// write byte
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte value)
        {
            this._stream.WriteByte(value);
        }
        /// <summary>
        /// write byte array
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException">value is null</exception>
        public void Write(byte[] value)
        {
            if (value == null) throw new ArgumentNullException("value");
            this._stream.Write(value, 0, value.Length);
        }
        /// <summary>
        /// write bool
        /// </summary>
        /// <param name="value"></param>
        public void Write(bool value)
        {
            this._stream.WriteByte(value ? (byte)1 : (byte)0);
        }
        /// <summary>
        /// write short
        /// </summary>
        /// <param name="value"></param>
        public void Write(short value)
        {
            this._tempBuffer[0] = (byte)(0xff & (value >> 8));
            this._tempBuffer[1] = (byte)(0xff & value);
            this._stream.Write(this._tempBuffer, 0, 2);
        }
        /// <summary>
        /// write int
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            this._tempBuffer[0] = (byte)(0xff & (value >> 24));
            this._tempBuffer[1] = (byte)(0xff & (value >> 16));
            this._tempBuffer[2] = (byte)(0xff & (value >> 8));
            this._tempBuffer[3] = (byte)(0xff & value);
            this._stream.Write(this._tempBuffer, 0, 4);
        }
        /// <summary>
        /// write long
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value)
        {
            this._tempBuffer[0] = (byte)(0xff & (value >> 56));
            this._tempBuffer[1] = (byte)(0xff & (value >> 48));
            this._tempBuffer[2] = (byte)(0xff & (value >> 40));
            this._tempBuffer[3] = (byte)(0xff & (value >> 32));
            this._tempBuffer[4] = (byte)(0xff & (value >> 24));
            this._tempBuffer[5] = (byte)(0xff & (value >> 16));
            this._tempBuffer[6] = (byte)(0xff & (value >> 8));
            this._tempBuffer[7] = (byte)(0xff & value);
            this._stream.Write(this._tempBuffer, 0, 8);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 获取 <see cref="BinaryWriter"/> 的基础流。
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