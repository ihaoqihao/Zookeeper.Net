using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// set watches request
    /// </summary>
    public sealed class SetWatchesRequest : IRecord
    {
        /// <summary>
        /// zxid
        /// </summary>
        public readonly long RelativeZxid;
        /// <summary>
        /// data watches
        /// </summary>
        public readonly string[] DataWatches;
        /// <summary>
        /// exist watches
        /// </summary>
        public readonly string[] ExistWatches;
        /// <summary>
        /// child watches
        /// </summary>
        public readonly string[] ChildWatches;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="relativeZxid"></param>
        /// <param name="dataWatches"></param>
        /// <param name="existWatches"></param>
        /// <param name="childWatches"></param>
        public SetWatchesRequest(long relativeZxid, string[] dataWatches, string[] existWatches, string[] childWatches)
        {
            this.RelativeZxid = relativeZxid;
            this.DataWatches = dataWatches;
            this.ExistWatches = existWatches;
            this.ChildWatches = childWatches;
        }

        /// <summary>
        /// write
        /// </summary>
        /// <param name="formatter"></param>
        public void Write(Utils.IFormatter formatter)
        {
            formatter.Write(this.RelativeZxid);
            formatter.Write(this.DataWatches);
            formatter.Write(this.ExistWatches);
            formatter.Write(this.ChildWatches);
        }
        /// <summary>
        /// read
        /// </summary>
        /// <param name="formatter"></param>
        public void Read(Utils.IFormatter formatter)
        {
            throw new NotImplementedException();
        }
    }
}