
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// create mode
    /// </summary>
    public sealed class CreateMode
    {
        /// <summary>
        /// flag
        /// </summary>
        public readonly int Flag;
        /// <summary>
        /// true表示临时的，false反之
        /// </summary>
        public readonly bool Ephemeral;
        /// <summary>
        /// true表示顺序
        /// </summary>
        public readonly bool Sequential;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="ephemeral"></param>
        /// <param name="sequential"></param>
        public CreateMode(int flag, bool ephemeral, bool sequential)
        {
            this.Flag = flag;
            this.Ephemeral = ephemeral;
            this.Sequential = sequential;
        }
    }

    /// <summary>
    /// create modes
    /// </summary>
    static public class CreateModes
    {
        /// <summary>
        /// 持久化
        /// </summary>
        static public readonly CreateMode Persistent = new CreateMode(0, false, false);
        /// <summary>
        /// 持久化+顺序
        /// </summary>
        static public readonly CreateMode PersistentSequential = new CreateMode(2, false, true);
        /// <summary>
        /// 临时
        /// </summary>
        static public readonly CreateMode Ephemeral = new CreateMode(1, true, false);
        /// <summary>
        /// 临时+顺序
        /// </summary>
        static public readonly CreateMode EphemeralSequential = new CreateMode(3, true, true);
    }
}