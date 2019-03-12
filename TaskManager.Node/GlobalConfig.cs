using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Node
{
    public static class GlobalConfig
    {
        private static string _redisServer;

        public static string RedisServer
        {
            get
            {
                if (string.IsNullOrEmpty(_redisServer))
                {
                    _redisServer = ConfigurationManager.AppSettings["RedisServer"];
                }
                return _redisServer;
            }
        }
        private static string _nodeID;

        public static int NodeID
        {
            get
            {
                if (string.IsNullOrEmpty(_nodeID))
                {
                    _nodeID = ConfigurationManager.AppSettings["NodeID"];
                }
                return Convert.ToInt32(_nodeID);
            }
        }
        public static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
                }
                return _connectionString;
            }
        }
        /// <summary>
        /// 任务数据库连接
        /// </summary>
        ///public static string TaskDataBaseConnectString { get; set; }
        /// <summary>
        /// 当前节点标识
        /// </summary>
        ///public static int NodeID { get; set; }
        /// <summary>
        /// 任务调度平台web url地址
        /// </summary>
        //public static string TaskManagerWebUrl { get { return System.Configuration.ConfigurationSettings.AppSettings["TaskManagerWebUrl"]; } }
        ///// <summary>
        ///// 任务dll根目录
        ///// </summary>
        //public static string TaskDllDir = "任务dll根目录";
        ///// <summary>
        ///// 任务dll本地版本缓存
        ///// </summary>
        //public static string TaskDllCompressFileCacheDir = "任务dll版本缓存";
        ///// <summary>
        ///// 任务平台共享程序集
        ///// </summary>
        //public static string TaskSharedDllsDir = "任务dll共享程序集";
        /// <summary>
        /// 任务平台节点使用的监控插件
        /// </summary>
        //public static List<SystemMonitor.BaseMonitor> Monitors = new List<SystemMonitor.BaseMonitor>();
        
    }
}
