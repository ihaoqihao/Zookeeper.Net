using System;
using Sodao.FastSocket.SocketBase.Utils;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// node keeper
    /// </summary>
    static public class NodeFactory
    {
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="node"></param>
        /// <param name="callback"></param>
        static public void TryEnsureCreate(IZookClient zk, NodeInfo node, Action callback)
        {
            TryEnsureCreate(zk, new NodeInfo[] { node }, callback);
        }
        /// <summary>
        /// 批量创建一组节点
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="nodes"></param>
        /// <param name="callback">当所有节点创建完毕时的回调</param>
        /// <exception cref="ArgumentNullException">zk is null.</exception>
        /// <exception cref="ArgumentNullException">nodes is null or empty.</exception>
        static public void TryEnsureCreate(IZookClient zk, NodeInfo[] nodes, Action callback)
        {
            if (zk == null) throw new ArgumentNullException("zk");
            if (nodes == null || nodes.Length == 0) throw new ArgumentNullException("nodes");
            TryEnsureCreate(zk, nodes, 0, callback);
        }
        /// <summary>
        /// 批量创建一组节点
        /// </summary>
        /// <param name="zk"></param>
        /// <param name="nodes"></param>
        /// <param name="index"></param>
        /// <param name="callback">当所有节点创建完毕时的回调</param>
        static private void TryEnsureCreate(IZookClient zk, NodeInfo[] nodes, int index, Action callback)
        {
            if (index >= nodes.Length)
            {
                if (callback != null) callback();
                return;
            }

            var currNode = nodes[index];
            zk.Create(currNode.Path, currNode.Data, currNode.ACL, currNode.CreateMode).ContinueWith(c =>
            {
                if (c.IsFaulted)
                {
                    var kex = c.Exception.InnerException as KeeperException;
                    if (kex == null || kex.Error != Data.ZoookError.NODEEXISTS)
                    {
                        TaskEx.Delay(new Random().Next(1000, 3000)).ContinueWith(_ =>
                            TryEnsureCreate(zk, nodes, index, callback));
                        return;
                    }
                }
                TryEnsureCreate(zk, nodes, index + 1, callback);
            });
        }
    }
}