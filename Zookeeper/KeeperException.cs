using System;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper exception
    /// </summary>
    public class KeeperException : ApplicationException
    {
        /// <summary>
        /// path
        /// </summary>
        public readonly string Path = null;
        /// <summary>
        /// error code
        /// </summary>
        public readonly Data.ZoookError Error;

        /// <summary>
        /// new
        /// </summary>
        /// <param name="error"></param>
        public KeeperException(Data.ZoookError error)
            : base(error.ToString())
        {
            this.Error = error;
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="path"></param>
        /// <param name="error"></param>
        public KeeperException(string path, Data.ZoookError error)
            : base(string.Concat("path:{", path ?? string.Empty, "}, error:{", error.ToString(), "}"))
        {
            this.Path = path;
            this.Error = error;
        }
    }
}