
namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// operation code
    /// </summary>
    public enum OpCode : int
    {
        /// <summary>
        /// 通知
        /// </summary>
        Notification = 0,
        /// <summary>
        /// 新建
        /// </summary>
        Create = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 检测是否存在
        /// </summary>
        Exists = 3,
        /// <summary>
        /// 获取数据
        /// </summary>
        GetData = 4,
        /// <summary>
        /// 设置数据
        /// </summary>
        SetData = 5,
        /// <summary>
        /// 获取acl
        /// </summary>
        GetACL = 6,
        /// <summary>
        /// 设置acl
        /// </summary>
        SetACL = 7,
        /// <summary>
        /// 获取children
        /// </summary>
        GetChildren = 8,
        /// <summary>
        /// 同步
        /// </summary>
        Sync = 9,
        /// <summary>
        /// ping, 心跳
        /// </summary>
        Ping = 11,
        /// <summary>
        /// 获取children v2
        /// </summary>
        GetChildren2 = 12,
        /// <summary>
        /// 认证
        /// </summary>
        Auth = 100,
        /// <summary>
        /// 设置侦听
        /// </summary>
        SetWatches = 101,
        /// <summary>
        /// 新建session
        /// </summary>
        CreateSession = -10,
        /// <summary>
        /// 关闭session
        /// </summary>
        CloseSession = -11,
        /// <summary>
        /// error
        /// </summary>
        Error = -1,
    }
}