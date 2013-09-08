using System;
using System.Threading.Tasks;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper client interface
    /// </summary>
    public interface IZookClient
    {
        /// <summary>
        /// keeper state changed event
        /// </summary>
        event KeeperStateChangedHandler KeeperStateChanged;

        /// <summary>
        /// get current keeperState
        /// </summary>
        Data.KeeperState CurrentKeeperState
        {
            get;
        }
        /// <summary>
        /// true表示禁用自动重新注册watch, 默认为false
        /// </summary>
        bool DisableAutoWatchReset
        {
            get;
            set;
        }
        /// <summary>
        /// Add the specified scheme:auth information to this connection.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="auth">The auth.</param>
        void AddAuthInfo(string scheme, byte[] auth);
        /// <summary>
        /// Specify the default watcher for the connection (overrides the one
        /// specified during construction).
        /// </summary>
        /// <param name="watcher">The watcher.</param>
        void Register(IWatcher watcher);
        /// <summary>
        /// Create a node with the given path. The node data will be the given data,
        /// and node acl will be the given acl.
        /// </summary>
        /// <param name="path">The path for the node.</param>
        /// <param name="data">The data for the node.</param>
        /// <param name="acl">The acl for the node.</param>
        /// <param name="createMode">specifying whether the node to be created is ephemeral and/or sequential.</param>
        /// <returns></returns>
        Task<string> Create(string path, byte[] data, Data.ACL[] acl, Data.CreateMode createMode);
        /// <summary>
        /// Delete the node with the given path. The call will succeed if such a node
        /// exists, and the given version matches the node's version (if the given
        /// version is -1, it matches any node's versions).
        ///
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if the nodes does not exist.
        ///
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        ///
        /// A KeeperException with error code KeeperException.NotEmpty will be thrown
        /// if the node has children.
        /// 
        /// This operation, if successful, will trigger all the watches on the node
        /// of the given path left by exists API calls, and the watches on the parent
        /// node left by getChildren API calls.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="version">The version.</param>
        Task Delete(string path, int version = -1);
        /// <summary>
        /// Return the stat of the node of the given path. Return null if no such a
        /// node exists.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch will be
        /// triggered by a successful operation that creates/delete the node or sets
        /// the data on the node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="watch">whether need to watch this node</param>
        /// <returns>the stat of the node of the given path; return null if no such a node exists.</returns>
        Task<Data.Stat> Exists(string path, bool watch);
        /// <summary>
        /// Return the stat of the node of the given path. Return null if no such a
        /// node exists.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch will be
        /// triggered by a successful operation that creates/delete the node or sets
        /// the data on the node.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="watcher">The watcher.</param>
        /// <returns>the stat of the node of the given path; return null if no such a node exists.</returns>
        Task<Data.Stat> Exists(string path, IWatcher watcher);
        /// <summary>
        /// Return the data and the stat of the node of the given path.
        /// </summary>
        /// <param name="path">the given path</param>
        /// <param name="watch">whether need to watch this node</param>
        /// <returns></returns>
        Task<Data.GetDataResponse> GetData(string path, bool watch);
        /// <summary>
        /// Return the data and the stat of the node of the given path.
        /// </summary>
        /// <param name="path">the given path</param>
        /// <param name="watcher">explicit watcher</param>
        /// <returns></returns>
        Task<Data.GetDataResponse> GetData(string path, IWatcher watcher);
        /// <summary>
        /// Set the data for the node of the given path if such a node exists and the
        /// given version matches the version of the node (if the given version is
        /// -1, it matches any node's versions). Return the stat of the node.
        /// 
        /// This operation, if successful, will trigger all the watches on the node
        /// of the given path left by getData calls.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// 
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        ///
        /// The maximum allowable size of the data array is 1 MB (1,048,576 bytes).
        /// Arrays larger than this will cause a KeeperExecption to be thrown.
        /// </summary>
        /// <param name="path">the path of the node</param>
        /// <param name="data">the data to set</param>
        /// <param name="version">the expected matching version</param>
        /// <returns>the state of the node</returns>
        Task<Data.Stat> SetData(string path, byte[] data, int version = -1);
        /// <summary>
        /// Return the ACL and stat of the node of the given path.
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path">the given path for the node</param>
        /// <returns></returns>
        Task<Data.GetACLResponse> GetACL(string path);
        /// <summary>
        /// Set the ACL for the node of the given path if such a node exists and the
        /// given version matches the version of the node. Return the stat of the
        /// node.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// 
        /// A KeeperException with error code KeeperException.BadVersion will be
        /// thrown if the given version does not match the node's version.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="acl"></param>
        /// <param name="version"></param>
        /// <returns>the stat of the node.</returns>
        Task<Data.Stat> SetACL(string path, Data.ACL[] acl, int version = -1);
        /// <summary>
        /// Return the list of the children of the node of the given path.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watch"></param>
        /// <returns>an unordered array of children of the node with the given path</returns>
        Task<string[]> GetChildren(string path, bool watch);
        /// <summary>
        /// Return the list of the children of the node of the given path.
        /// 
        /// If the watch is non-null and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watcher"></param>
        /// <returns>an unordered array of children of the node with the given path</returns>
        Task<string[]> GetChildren(string path, IWatcher watcher);
        /// <summary>
        /// For the given znode path return the stat and children list.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watch"></param>
        /// <returns></returns>
        Task<Data.GetChildren2Response> GetChildren2(string path, bool watch);
        /// <summary>
        /// For the given znode path return the stat and children list.
        /// 
        /// If the watch is true and the call is successful (no exception is thrown),
        /// a watch will be left on the node with the given path. The watch willbe
        /// triggered by a successful operation that deletes the node of the given
        /// path or creates/delete a child under the node.
        /// 
        /// The list of children returned is not sorted and no guarantee is provided
        /// as to its natural or lexical order.
        /// 
        /// A KeeperException with error code KeeperException.NoNode will be thrown
        /// if no node with the given path exists.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="watcher"></param>
        /// <returns></returns>
        Task<Data.GetChildren2Response> GetChildren2(string path, IWatcher watcher);
    }
}