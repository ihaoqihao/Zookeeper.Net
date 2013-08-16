using System;
using System.Configuration;
using System.IO;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zookeeper client factory
    /// </summary>
    static public class ZookClientFactory
    {
        /// <summary>
        /// create new zook client
        /// </summary>
        /// <param name="chrootPath"></param>
        /// <param name="connectionString"></param>
        /// <param name="sessionTimeout"></param>
        /// <returns></returns>
        static public IZookClient Create(string chrootPath, string connectionString, TimeSpan sessionTimeout)
        {
            return new ZookClient(chrootPath, connectionString, sessionTimeout);
        }
        /// <summary>
        /// create new zook client
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        static public IZookClient Create(Config.ZKClientConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            return new ZookClient(config.Chroot, config.ConnectionString, TimeSpan.FromMilliseconds(config.SessionTimeout));
        }
        /// <summary>
        /// create new zook client
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">sectionName is null or empty</exception>
        /// <exception cref="ArgumentNullException">clientName is null or empty</exception>
        /// <exception cref="InvalidOperationException">clientName不存在</exception>
        static public IZookClient Create(string configPath, string sectionName, string clientName)
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");
            if (string.IsNullOrEmpty(clientName)) throw new ArgumentNullException("clientName");

            Config.ZookeeperConfig keeperConfig = null;
            if (string.IsNullOrEmpty(configPath)) keeperConfig = ConfigurationManager.GetSection(sectionName) as Config.ZookeeperConfig;
            else
            {
                keeperConfig = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath)
                }, ConfigurationUserLevel.None).GetSection(sectionName) as Config.ZookeeperConfig;
            }

            foreach (Config.ZKClientConfig clientConfig in keeperConfig.Clients)
                if (clientConfig.Name == clientName) return Create(clientConfig);

            throw new InvalidOperationException("clientName不存在");
        }
        /// <summary>
        /// create new zook client
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        static public IZookClient Create(string clientName)
        {
            return Create(null, "zookeeper", clientName);
        }
    }
}