using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sodao.Zookeeper.Utils
{
    /// <summary>
    /// Binary Formatter
    /// </summary>
    public sealed class BinaryFormatter : IFormatter
    {
        #region Private Members
        private BinaryReader _reader = null;
        private BinaryWriter _writer = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        public BinaryFormatter(Stream inputStream, Stream outputStream)
        {
            if (inputStream != null) this._reader = new BinaryReader(inputStream);
            if (outputStream != null) this._writer = new BinaryWriter(outputStream);
        }
        #endregion

        #region IFormatter Members
        /// <summary>
        /// read byte
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return this._reader.ReadByte();
        }
        /// <summary>
        /// read buffer
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBuffer()
        {
            int length = this._reader.ReadInt32();
            if (length < 1) return null;
            return this._reader.ReadBuffer(length);
        }

        /// <summary>
        /// read bool
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            return this._reader.ReadBool();
        }
        /// <summary>
        /// read int16
        /// </summary>
        /// <returns></returns>
        public int ReadInt16()
        {
            return this._reader.ReadInt16();
        }
        /// <summary>
        /// read int32
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            return this._reader.ReadInt32();
        }
        /// <summary>
        /// read int64
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            return this._reader.ReadInt64();
        }
        /// <summary>
        /// read string
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int length = this._reader.ReadInt32();
            if (length < 1) return null;
            return Encoding.UTF8.GetString(this._reader.ReadBuffer(length));
        }
        /// <summary>
        /// read string array
        /// </summary>
        /// <returns></returns>
        public string[] ReadStringArray()
        {
            var len = this.ReadInt32();
            if (len < 0) return null;

            var arr = new string[len];
            for (int i = 0; i < len; i++) arr[i] = this.ReadString();
            return arr;
        }
        /// <summary>
        /// write record
        /// </summary>
        /// <typeparam name="TRecord"></typeparam>
        /// <returns></returns>
        public TRecord ReadRecord<TRecord>() where TRecord : Data.IRecord, new()
        {
            var obj = new TRecord();
            obj.Read(this);
            return obj;
        }
        /// <summary>
        /// read recore list
        /// </summary>
        /// <typeparam name="TRecord"></typeparam>
        /// <returns></returns>
        public TRecord[] ReadRecordList<TRecord>() where TRecord : Data.IRecord, new()
        {
            int length = this._reader.ReadInt32();
            if (length < 1) return null;

            var arr = new TRecord[length];
            for (int i = 0; i < length; i++)
            {
                var objRecord = new TRecord();
                objRecord.Read(this);
                arr[i] = objRecord;
            }
            return arr;
        }

        /// <summary>
        /// write byte
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte value)
        {
            this._writer.Write(value);
        }
        /// <summary>
        /// write byte array
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte[] value)
        {
            if (value == null) { this._writer.Write(-1); return; }
            this._writer.Write(value.Length);
            this._writer.Write(value);
        }

        /// <summary>
        /// write bool
        /// </summary>
        /// <param name="value"></param>
        public void Write(bool value)
        {
            this._writer.Write(value);
        }
        /// <summary>
        /// write short
        /// </summary>
        /// <param name="value"></param>
        public void Write(short value)
        {
            this._writer.Write(value);
        }
        /// <summary>
        /// write int
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            this._writer.Write(value);
        }
        /// <summary>
        /// write long
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value)
        {
            this._writer.Write(value);
        }
        /// <summary>
        /// write string
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            if (value == null) { this._writer.Write(-1); return; }
            var payload = System.Text.Encoding.UTF8.GetBytes(value);
            this._writer.Write(payload.Length);
            this._writer.Write(payload);
        }
        /// <summary>
        /// write string array
        /// </summary>
        /// <param name="strs"></param>
        public void Write(string[] strs)
        {
            if (strs == null) { this._writer.Write(-1); return; }
            this._writer.Write(strs.Length);
            foreach (var str in strs) this.Write(str);
        }
        /// <summary>
        /// write record
        /// </summary>
        /// <param name="record"></param>
        /// <exception cref="ArgumentNullException">record is null</exception>
        public void Write(Data.IRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");
            record.Write(this);
        }
        /// <summary>
        /// write record list
        /// </summary>
        /// <param name="records"></param>
        /// <exception cref="ArgumentNullException">records.Item is null</exception>
        public void Write(IEnumerable<Data.IRecord> records)
        {
            if (records == null) { this._writer.Write(-1); return; }

            this._writer.Write(records.Count());
            foreach (var record in records)
            {
                if (record == null) throw new ArgumentNullException("records.Item");
                record.Write(this);
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            if (this._reader != null)
            {
                this._reader.Dispose();
                this._reader = null;
            }
            if (this._writer != null)
            {
                this._writer.Dispose();
                this._writer = null;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// start write
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        static public BinaryFormatter StartWrite(Stream stream)
        {
            return new BinaryFormatter(null, stream);
        }
        /// <summary>
        /// start read
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        static public BinaryFormatter StartRead(Stream stream)
        {
            return new BinaryFormatter(stream, null);
        }
        #endregion
    }
}