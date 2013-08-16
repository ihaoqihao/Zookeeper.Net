using System.IO;
using Sodao.Zookeeper.Data;

namespace Sodao.Zookeeper.Utils
{
    /// <summary>
    /// Marshaller
    /// </summary>
    static public class Marshaller
    {
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="record"></param>
        /// <param name="writeLength"></param>
        /// <returns></returns>
        static public byte[] Serialize(IRecord record, bool writeLength = true)
        {
            var stream = new MemoryStream();
            using (var writer = BinaryFormatter.StartWrite(stream))
            {
                if (writeLength) { writer.Write(-1); }
                if (record != null) { record.Write(writer); }
                if (writeLength)
                {
                    stream.Position = 0;
                    writer.Write((int)stream.Length - 4);
                }

                return stream.ToArray();
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="xid"></param>
        /// <param name="code"></param>
        /// <param name="record"></param>
        /// <param name="writeLength"></param>
        /// <returns></returns>
        static public byte[] Serialize(int xid, OpCode code, IRecord record, bool writeLength = true)
        {
            var stream = new MemoryStream();
            using (var writer = BinaryFormatter.StartWrite(stream))
            {
                if (writeLength) { writer.Write(-1); }
                writer.Write(xid);
                writer.Write((int)code);
                if (record != null) { record.Write(writer); }
                if (writeLength)
                {
                    stream.Position = 0;
                    writer.Write((int)stream.Length - 4);
                }

                return stream.ToArray();
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static public T Deserialize<T>(byte[] bytes) where T : IRecord, new()
        {
            var obj = new T();

            var stream = new MemoryStream(bytes);
            using (var reader = BinaryFormatter.StartRead(stream))
            {
                obj.Read(reader);
            }

            return obj;
        }
    }
}