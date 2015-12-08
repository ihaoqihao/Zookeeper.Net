using System;
using System.Threading.Tasks;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// node creator
    /// </summary>
    static public class NodeCreator
    {
        /// <summary>
        /// try create node
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">zk is null.</exception>
        /// <exception cref="ArgumentNullException">node is null.</exception>
        static public Task TryCreate(IZookClient zk, NodeInfo node)
        {
            if (zk == null) throw new ArgumentNullException("zk");
            if (node == null) throw new ArgumentNullException("node");

            return zk.Create(node.Path, node.Data, node.ACL, node.CreateMode).ContinueWith(c =>
            {
                var ex = c.Exception.InnerException as KeeperException;
                if (ex != null && ex.Error == Data.ZoookError.NODEEXISTS) return;
                throw c.Exception.InnerException;
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /// <summary>
        /// try create nodes
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        static public Task TryCreate(IZookClient zk, NodeInfo[] nodes)
        {
            if (zk == null) throw new ArgumentNullException("zk");
            if (nodes == null || nodes.Length == 0) throw new ArgumentNullException("nodes");

            var source = new TaskCompletionSource<bool>();
            TryCreate(zk, nodes, 0, source);
            return source.Task;
        }
        /// <summary>
        /// try create nodes
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="nodes"></param>
        /// <param name="index"></param>
        /// <param name="source"></param>
        static private void TryCreate(IZookClient zk, NodeInfo[] nodes, int index, TaskCompletionSource<bool> source)
        {
            if (index >= nodes.Length)
            {
                source.TrySetResult(true);
                return;
            }

            var node = nodes[index];
            zk.Create(node.Path, node.Data, node.ACL, node.CreateMode).ContinueWith(c =>
            {
                if (c.IsFaulted)
                {
                    var ex = c.Exception.InnerException as KeeperException;
                    if (ex == null || ex.Error != Data.ZoookError.NODEEXISTS)
                    {
                        source.TrySetException(c.Exception.InnerException);
                        return;
                    }
                }
                TryCreate(zk, nodes, index + 1, source);
            });
        }
    }
}