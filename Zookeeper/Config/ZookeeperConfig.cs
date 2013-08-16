using System.Configuration;

namespace Sodao.Zookeeper.Config
{
    /// <summary>
    /// zookeeper config
    /// </summary>
    public sealed class ZookeeperConfig : ConfigurationSection
    {
        /// <summary>
        /// zk clients
        /// </summary>
        [ConfigurationProperty("clients", IsRequired = true)]
        public ZKClientCollection Clients
        {
            get { return this["clients"] as ZKClientCollection; }
        }
    }
}