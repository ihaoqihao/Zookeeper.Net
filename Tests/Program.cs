using System;
using Sodao.Zookeeper;
using Sodao.Zookeeper.Data;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Sodao.FastSocket.SocketBase.Log.Trace.EnableConsole();
            Sodao.FastSocket.SocketBase.Log.Trace.EnableDiagnostic();

            var client = Sodao.Zookeeper.ZookClientPool.Get("zk1");
            client.Register(new WatcherWrapper(e =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Path);
                Console.WriteLine(e.Type.ToString());
                Console.WriteLine(e.State.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
            }));

            var watcher = new ChildrenWatcher(client, "/", c =>
            {
                Console.WriteLine(string.Join("-", c));
            });

            Console.ReadLine();
            var sessionNode = new SessionNode(client, "/tempABC", null, IDs.OPEN_ACL_UNSAFE);
            Console.ReadLine();
            sessionNode.Close();

            Console.ReadLine();
        }
    }
}