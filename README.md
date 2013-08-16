# Overview

A c# client for Apache Zookeeper.

# Example

```csharp
var client = Sodao.Zookeeper.ZookClientPool.Get("zk1");

//create a node
client.Create("/test1", null, IDs.OPEN_ACL_UNSAFE, CreateModes.Ephemeral).ContinueWith(c =>
{
    if (c.IsFaulted) Console.Write(string.Concat("error: ", c.Exception.ToString()));
    else Console.WriteLine(c.Result);
});
//get child nodes
client.GetChildren("/", false).ContinueWith(c =>
{
    if (c.IsFaulted) Console.Write(string.Concat("error: ", c.Exception.ToString()));
    else
    {
        foreach (var nodeName in c.Result)
            Console.WriteLine(nodeName);
    }
});

//remove /test1 node
Sodao.FastSocket.SocketBase.Utils.TaskEx.Delay(3000, () =>
{
    client.Delete("/test1").ContinueWith(c =>
    {
        if (c.IsFaulted) Console.Write(string.Concat("error: ", c.Exception.ToString()));
        else Console.WriteLine("/test1 was deleted");
    });
});

//session node
var sessionNode = new SessionNode(client, "/sessionNode1", null, IDs.OPEN_ACL_UNSAFE);
Console.ReadLine();
sessionNode.Close();
Console.WriteLine("session node closed");
```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>

  <configSections>
    <section name="zookeeper"
             type="Sodao.Zookeeper.Config.ZookeeperConfig, Zookeeper"/>
  </configSections>

  <zookeeper>
    <clients>
      <client name="zk1"
              chroot=""
              sessionTimeout="5000"
              connectionString="127.0.0.1:2181" />
    </clients>
  </zookeeper>

</configuration>
```