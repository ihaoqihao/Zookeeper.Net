using System;
using Sodao.Zookeeper;
using Sodao.Zookeeper.Data;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sodao.FastSocket.SocketBase.Log.Trace.EnableConsole();
            //Sodao.FastSocket.SocketBase.Log.Trace.EnableDiagnostic();

            var client = Sodao.Zookeeper.ZookClientPool.Get("zk1");
            client.Register(new WatcherAction(e =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Path);
                Console.WriteLine(e.Type.ToString());
                Console.WriteLine(e.State.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
            }));

            Console.WriteLine("watch zk node /hong2...");
            var watcher = new ChildrenWatcher(client, "/hong2", c =>
            {
                Console.WriteLine(string.Join("-", c));
            });

            Console.WriteLine("create zk session node /hong2/tempABC...");
            var sessionNode = new SessionNode(client, "/hong2/tempABC", null, IDs.OPEN_ACL_UNSAFE);

            Console.WriteLine("create zk node /hong2...");
            NodeCreator.TryCreate(client, new NodeInfo("/hong2", null, IDs.OPEN_ACL_UNSAFE, CreateModes.Persistent));

            Console.WriteLine("press any key stop thrift client...");
            Console.ReadLine();
            client.Stop();

            Console.WriteLine("press any key start thrift client...");
            Console.ReadLine();
            client.Start();

            Console.WriteLine("press any key dispose zk node(/hong2, /hong2/tempABC)...");
            Console.ReadLine();
            sessionNode.Dispose();
            client.Delete("/hong2");

            Console.WriteLine("press any key exit...");
            Console.ReadLine();
        }
    }
}