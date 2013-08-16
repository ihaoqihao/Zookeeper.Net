
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// a record interface
    /// </summary>
    public interface IRecord
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="formatter"></param>
        void Write(Utils.IFormatter formatter);
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="formatter"></param>
        void Read(Utils.IFormatter formatter);
    }
}