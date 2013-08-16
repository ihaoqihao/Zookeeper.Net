using System;

namespace Sodao.Zookeeper.Data
{
    /// <summary>
    /// 权限枚举
    /// </summary>
    [Flags]
    public enum Perms : int
    {
        /// <summary>
        /// 读
        /// </summary>
        READ = 1 << 0,
        /// <summary>
        /// 写
        /// </summary>
        WRITE = 1 << 1,
        /// <summary>
        /// 创建
        /// </summary>
        CREATE = 1 << 2,
        /// <summary>
        /// 删除
        /// </summary>
        DELETE = 1 << 3,
        /// <summary>
        /// 管理员
        /// </summary>
        ADMIN = 1 << 4,
        /// <summary>
        /// all
        /// </summary>
        ALL = READ | WRITE | CREATE | DELETE | ADMIN
    }
}