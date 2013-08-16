using System.Configuration;

namespace Sodao.Zookeeper.Config
{
    /// <summary>
    /// zkclient collection
    /// </summary>
    [ConfigurationCollection(typeof(ZKClientConfig), AddItemName = "client")]
    public sealed class ZKClientCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 创建新元素。
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ZKClientConfig();
        }
        /// <summary>
        /// 获取指定元素的Key。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var zkClient = element as ZKClientConfig;
            return zkClient.Name;
        }
        /// <summary>
        /// 获取指定位置的对象。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public ZKClientConfig this[int i]
        {
            get { return BaseGet(i) as ZKClientConfig; }
        }
    }
}