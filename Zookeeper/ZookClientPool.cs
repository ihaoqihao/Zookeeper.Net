using System.Collections.Generic;
using System.Threading;

namespace Sodao.Zookeeper
{
    /// <summary>
    /// zook client pool
    /// </summary>
    static public class ZookClientPool
    {
        /// <summary>
        /// key:configPath+sectionName+clientName
        /// </summary>
        static private readonly Dictionary<string, IZookClient> _dicClients = new Dictionary<string, IZookClient>();
        static private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        /// <summary>
        /// get zook client
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        static public IZookClient Get(string clientName)
        {
            return Get(null, "zookeeper", clientName);
        }
        /// <summary>
        /// get zook client
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="sectionName"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        static public IZookClient Get(string configPath, string sectionName, string clientName)
        {
            string key = string.Concat(configPath ?? string.Empty, sectionName, clientName);

            _locker.EnterReadLock();
            try
            {
                IZookClient client = null;
                if (_dicClients.TryGetValue(key, out client)) return client;
            }
            finally { _locker.ExitReadLock(); }

            _locker.EnterWriteLock();
            try
            {
                IZookClient client = null;
                if (_dicClients.TryGetValue(key, out client)) return client;

                client = ZookClientFactory.Create(configPath, sectionName, clientName);
                _dicClients[key] = client;
                return client;
            }
            finally { _locker.ExitWriteLock(); }
        }
    }
}