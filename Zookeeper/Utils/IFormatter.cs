using System;
using System.Collections.Generic;

namespace Sodao.Zookeeper.Utils
{
    /// <summary>
    /// formatter
    /// </summary>
    public interface IFormatter : IDisposable
    {
        /// <summary>
        /// read byte
        /// </summary>
        /// <returns></returns>
        byte ReadByte();
        /// <summary>
        /// read buffer
        /// </summary>
        /// <returns></returns>
        byte[] ReadBuffer();
        /// <summary>
        /// read bool
        /// </summary>
        /// <returns></returns>
        bool ReadBool();
        /// <summary>
        /// read int16
        /// </summary>
        /// <returns></returns>
        int ReadInt16();
        /// <summary>
        /// read int32
        /// </summary>
        /// <returns></returns>
        int ReadInt32();
        /// <summary>
        /// read int64
        /// </summary>
        /// <returns></returns>
        long ReadInt64();
        /// <summary>
        /// read string
        /// </summary>
        /// <returns></returns>
        string ReadString();
        /// <summary>
        /// read string array
        /// </summary>
        /// <returns></returns>
        string[] ReadStringArray();
        /// <summary>
        /// read <see cref="Data.IRecord"/>
        /// </summary>
        /// <typeparam name="TRecord"></typeparam>
        /// <returns></returns>
        TRecord ReadRecord<TRecord>() where TRecord : Data.IRecord, new();
        /// <summary>
        /// read <see cref="Data.IRecord"/> list
        /// </summary>
        /// <typeparam name="TRecord"></typeparam>
        /// <returns></returns>
        TRecord[] ReadRecordList<TRecord>() where TRecord : Data.IRecord, new();

        /// <summary>
        /// write byte
        /// </summary>
        /// <param name="value"></param>
        void Write(byte value);
        /// <summary>
        /// write byte array
        /// </summary>
        /// <param name="value"></param>
        void Write(byte[] value);
        /// <summary>
        /// write bool
        /// </summary>
        /// <param name="value"></param>
        void Write(bool value);
        /// <summary>
        /// write short
        /// </summary>
        /// <param name="value"></param>
        void Write(short value);
        /// <summary>
        /// write int
        /// </summary>
        /// <param name="value"></param>
        void Write(int value);
        /// <summary>
        /// write long
        /// </summary>
        /// <param name="value"></param>
        void Write(long value);
        /// <summary>
        /// write string
        /// </summary>
        /// <param name="value"></param>
        void Write(string value);
        /// <summary>
        /// write string array
        /// </summary>
        /// <param name="strs"></param>
        void Write(string[] strs);
        /// <summary>
        /// write record
        /// </summary>
        /// <param name="record"></param>
        void Write(Data.IRecord record);
        /// <summary>
        /// write record list
        /// </summary>
        /// <param name="records"></param>
        void Write(IEnumerable<Data.IRecord> records);
    }
}