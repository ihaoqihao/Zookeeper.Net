using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// watcher event
    /// </summary>
    public sealed class WatcherEvent : IRecord
    {
        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        public WatcherEvent()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// type
        /// </summary>
        public EventType Type
        {
            get;
            private set;
        }
        /// <summary>
        /// state
        /// </summary>
        public KeeperState State
        {
            get;
            private set;
        }
        /// <summary>
        /// path
        /// </summary>
        public string Path
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
            this.Type = (EventType)formatter.ReadInt32();
            this.State = (KeeperState)formatter.ReadInt32();
            this.Path = formatter.ReadString();
        }
        #endregion
    }
}