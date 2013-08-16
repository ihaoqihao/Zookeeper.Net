using System;

namespace Sodao.Zookeeper.Utils
{
    /// <summary>
    /// path utils
    /// </summary>
    static public class PathUtils
    {
        /// <summary>
        /// validate the provided znode path string
        /// </summary>
        /// <param name="path">znode path string</param>
        /// <param name="isSequential">if the path is being created</param>
        static public void ValidatePath(string path, bool isSequential)
        {
            ValidatePath(isSequential ? path + "1" : path);
        }
        /// <summary>
        /// Validate the provided znode path string
        /// </summary>
        /// <param name="path">znode path string</param>
        /// <exception cref="ArgumentException">the path is invalid</exception>
        static public void ValidatePath(string path)
        {
            if (path == null) throw new ArgumentException("Path cannot be null");
            if (path.Length == 0) throw new ArgumentException("Path length must be > 0");
            if (path[0] != '/') throw new ArgumentException("Path must start with / character");
            if (path.Length == 1) return;// done checking - it's the root
            if (path[path.Length - 1] == '/') throw new ArgumentException("Path must not end with / character");

            string reason = null;
            char lastc = '/';
            char[] chars = path.ToCharArray();
            char c;
            for (int i = 1; i < chars.Length; lastc = chars[i], i++)
            {
                c = chars[i];

                if (c == 0) { reason = "null character not allowed @" + i; break; }
                else if (c == '/' && lastc == '/') { reason = "empty node name specified @" + i; break; }
                else if (c == '.' && lastc == '.')
                {
                    if (chars[i - 2] == '/' && ((i + 1 == chars.Length) || chars[i + 1] == '/'))
                    {
                        reason = "relative paths not allowed @" + i;
                        break;
                    }
                }
                else if (c == '.')
                {
                    if (chars[i - 1] == '/' && ((i + 1 == chars.Length) || chars[i + 1] == '/'))
                    {
                        reason = "relative paths not allowed @" + i;
                        break;
                    }
                }
                else if (c > '\u0000' && c < '\u001f' || c > '\u007f' && c < '\u009F'
                    || c > '\ud800' && c < '\uf8ff' || c > '\ufff0' && c < '\uffff')
                {
                    reason = "invalid charater @" + i;
                    break;
                }
            }

            if (reason != null) throw new ArgumentException(string.Format("Invalid path string \"{0}\" caused by {1}", path, reason));
        }

        /// <summary>
        /// Prepend the chroot to the client path (if present). The expectation of
        /// this function is that the client path has been validated before this
        /// function is called
        /// </summary>
        /// <param name="chroot"></param>
        /// <param name="clientPath">The path to the node.</param>
        /// <returns>server view of the path (chroot prepended to client path)</returns>
        static public string PrependChroot(string chroot, string clientPath)
        {
            if (string.IsNullOrEmpty(chroot)) return clientPath;
            // handle clientPath = "/"
            return clientPath.Length == 1 ? chroot : string.Concat(chroot, clientPath);
        }
        /// <summary>
        /// remove chroot
        /// </summary>
        /// <param name="chroot"></param>
        /// <param name="serverPath"></param>
        /// <returns></returns>
        static public string RemoveChroot(string chroot, string serverPath)
        {
            if (string.IsNullOrEmpty(chroot)) return serverPath;
            if (serverPath.CompareTo(chroot) == 0) return "/";
            else return serverPath.Substring(chroot.Length);
        }
    }
}