
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// ids
    /// </summary>
    static public class IDs
    {
        /// <summary>
        /// This Id represents anyone.
        /// </summary>
        static public readonly ZookID ANYONE_ID_UNSAFE = new ZookID("world", "anyone");
        /// <summary>
        /// This Id is only usable to set ACLs. It will get substituted with the
        /// Id's the client authenticated with.
        /// </summary>
        static public readonly ZookID AUTH_IDS = new ZookID("auth", "");

        /// <summary>
        /// This is a completely open ACL .
        /// </summary>
        static public readonly ACL[] OPEN_ACL_UNSAFE = new[] { new ACL(Perms.ALL, ANYONE_ID_UNSAFE) };
        /// <summary>
        /// This ACL gives the creators authentication id's all permissions.
        /// </summary>
        static public readonly ACL[] CREATOR_ALL_ACL = new[] { new ACL(Perms.ALL, AUTH_IDS) };
        /// <summary>
        /// This ACL gives the world the ability to read.
        /// </summary>
        static public readonly ACL[] READ_ACL_UNSAFE = new[] { new ACL(Perms.READ, ANYONE_ID_UNSAFE) };
    }
}