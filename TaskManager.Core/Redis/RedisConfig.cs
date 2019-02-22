using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Core.Redis
{
    public class RedisConfig
    {
        /// <summary>
        /// 内部Redis发布订阅通讯最大连接池
        /// </summary>
        public static int Redis_MaxConnectPoolSize = 2;
        /// <summary>
        /// 内部Redis发布订阅通讯通道名
        /// </summary>
        public static string Redis_Channel = "TaskManager.Redis.Channel";
        /// <summary>
        /// Redis发布订阅通讯通道注册失败间隔重试时间
        /// </summary>
        public static int Redis_Subscribe_FailConnect_ReConnect_Every_Time = 5;
        /// <summary>
        /// RedisServer
        /// </summary>
        public static string RedisServer;
        /// <summary>
        /// 配置中RedisServer的Key名称
        /// </summary>
        public static string RedisServerKey = "RedisServer";
    }
}
