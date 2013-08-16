using System.Configuration;

namespace Sodao.Zookeeper.Config
{
    /// <summary>
    /// zk client config
    /// </summary>
    public sealed class ZKClientConfig : ConfigurationElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }
        /// <summary>
        /// 根目录
        /// </summary>
        [ConfigurationProperty("chroot", IsRequired = false, DefaultValue = null)]
        public string Chroot
        {
            get { return (string)this["chroot"]; }
        }
        /// <summary>
        /// session超时时间，单位：毫秒数
        /// </summary>
        [ConfigurationProperty("sessionTimeout", IsRequired = true, DefaultValue = 10000)]
        public int SessionTimeout
        {
            get { return (int)this["sessionTimeout"]; }
        }
        /// <summary>
        /// zk集群连接字符串，如127.0.0.1:2181,127.0.0.1:2182(多个server之间用,隔开)
        /// </summary>
        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
        }
    }
}