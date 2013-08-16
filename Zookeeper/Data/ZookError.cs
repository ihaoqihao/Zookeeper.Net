
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// Codes which represent the various KeeperException
    /// types. This enum replaces the deprecated earlier static final int
    /// constants. The old, deprecated, values are in "camel case" while the new
    /// enum values are in all CAPS.
    /// </summary>
    public enum ZoookError
    {
        /// <summary>
        /// Everything is OK
        /// </summary>
        OK = 0,
        /// <summary>
        /// System and server-side errors.
        /// This is never thrown by the server, it shouldn't be used other than
        /// to indicate a range. Specifically error codes greater than this
        /// value, but lesser than {@link #APIERROR}, are system errors.
        /// </summary>
        SYSTEMERROR = -1,
        /// <summary>
        /// A runtime inconsistency was found 
        /// </summary>
        RUNTIMEINCONSISTENCY = -2,
        /// <summary>
        /// A data inconsistency was found 
        /// </summary>
        DATAINCONSISTENCY = -3,
        /// <summary>
        /// Connection to the server has been lost
        /// </summary>
        CONNECTIONLOSS = -4,
        /// <summary>
        /// Error while marshalling or unmarshalling data
        /// </summary>
        MARSHALLINGERROR = -5,
        /// <summary>
        /// Operation is unimplemented
        /// </summary>
        UNIMPLEMENTED = -6,
        /// <summary>
        /// Operation timeout
        /// </summary>
        OPERATIONTIMEOUT = -7,
        /// <summary>
        /// Invalid arguments
        /// </summary>
        BADARGUMENTS = -8,
        /// <summary>
        /// API errors.
        /// This is never thrown by the server, it shouldn't be used other than
        /// to indicate a range. Specifically error codes greater than this
        /// value are API errors (while values less than this indicate a
        /// {@link #SYSTEMERROR}).
        /// </summary>
        APIERROR = -100,
        /// <summary>
        /// Node does not exist 
        /// </summary>
        NONODE = -101,
        /// <summary>
        /// Not authenticated
        /// </summary>
        NOAUTH = -102,
        /// <summary>
        /// Version conflict
        /// </summary>
        BADVERSION = -103,
        /// <summary>
        /// Ephemeral nodes may not have children
        /// </summary>
        NOCHILDRENFOREPHEMERALS = -108,
        /// <summary>
        /// The node already exists
        /// </summary>
        NODEEXISTS = -110,
        /// <summary>
        /// The node has children
        /// </summary>
        NOTEMPTY = -111,
        /// <summary>
        /// The session has been expired by the server
        /// </summary>
        SESSIONEXPIRED = -112,
        /// <summary>
        /// Invalid callback specified
        /// </summary>
        INVALIDCALLBACK = -113,
        /// <summary>
        /// Invalid ACL specified
        /// </summary>
        INVALIDACL = -114,
        /// <summary>
        /// Client authentication failed
        /// </summary>
        AUTHFAILED = -115,
        /// <summary>
        /// Session moved to another server, so operation is ignored
        /// </summary>
        SESSIONMOVED = -118
    }
}