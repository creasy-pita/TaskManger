using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.Config
{
    /// <summary>
    /// 项目全局配置
    /// </summary>
    public static class BSFConfig
    {
        /// <summary>
        /// 是否写错误日志
        /// </summary>
        public static bool IsWriteErrorLog { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteErrorLog", "true")); } }
        /// <summary>
        /// 是否拦截访问日志
        /// </summary>
        public static bool IsWriteVisitLog { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteVisitLog", "false")); } }
        /// <summary>
        /// 错误日志是否写入监控平台
        /// </summary>
        public static bool IsWriteErrorLogToMonitorPlatform { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteErrorLogToMonitorPlatform", "false")); } }
        /// <summary>
        /// 错误日志是否写入本地文件
        /// </summary>
        public static bool IsWriteErrorLogToLocalFile { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteErrorLogToLocalFile", "true")); } }
        /// <summary>
        /// 是否写常用日志
        /// </summary>
        public static bool IsWriteCommonLog { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteCommonLog", "true")); } }
        /// <summary>
        /// 常用日志是否写入监控平台
        /// </summary>
        public static bool IsWriteCommonLogToMonitorPlatform { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteCommonLogToMonitorPlatform", "false")); } }
        /// <summary>
        /// 常用日志是否写入本地文件
        /// </summary>
        public static bool IsWriteCommonLogToLocalFile { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteCommonLogToLocalFile", "true")); } }
        /// <summary>
        /// 是否写耗时日志
        /// </summary>
        public static bool IsWriteTimeWatchLog { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteTimeWatchLog", "false")); } }
        /// <summary>
        /// 耗时日志是否写入监控平台
        /// </summary>
        public static bool IsWriteTimeWatchLogToMonitorPlatform { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteTimeWatchLogToMonitorPlatform", "false")); } }
        /// <summary>
        /// 耗时日志是否写入本地文件
        /// </summary>
        public static bool IsWriteTimeWatchLogToLocalFile { get { return Convert.ToBoolean(ConfigHelper.Get("IsWriteTimeWatchLogToLocalFile", "false")); } }
        /// <summary>
        /// 耗时监控数据库连接
        /// </summary>
        public static string TimeWatchConnectionString { get { return ConfigHelper.Get("TimeWatchConnectionString", ""); } }
        /// <summary>
        /// 监控平台数据库连接
        /// </summary>
        public static string MonitorPlatformConnectionString { get { return string.IsNullOrEmpty(monitorPlatformConnectionString) ? ConfigHelper.Get("MonitorPlatformConnectionString", "") : monitorPlatformConnectionString; } set { monitorPlatformConnectionString = value; } }//server=192.168.17.200;Initial Catalog=bs_MonitorPlatform;User ID=sa;Password=Xx~!@#;
        private static string monitorPlatformConnectionString;

        /// <summary>
        /// 统一配置中心数据库连接
        /// </summary>
        public static string ConfigManagerConnectString = ConfigHelper.Get("ConfigManagerConnectString", "");//server=192.168.17.200;Initial Catalog=dyd_new_main;User ID=sa;Password=Xx~!@#;

        /// <summary>
        /// 当前项目名称
        /// </summary>
        public static string ProjectName = ConfigHelper.Get("ProjectName", "未命名项目");

        /// <summary>
        /// 当前项目默认开发人员
        /// </summary>
        public static string ProjectDeveloper { get { return ConfigHelper.Get("ProjectDeveloper", ""); } }


        /// <summary>
        /// 集群性能监控库连接
        /// </summary>
        public static string ClusterConnectString { get { return ConfigHelper.Get("ClusterConnectString", ""); } }

        /// <summary>
        /// 集群性能监控库连接
        /// </summary>
        public static string PlatformManageConnectString { get { return ConfigHelper.Get("PlatformManageConnectString", ""); } }

        /// <summary>
        /// 耗时库连接
        /// </summary>
        public static string TimeWatchConnectString { get { return ConfigHelper.Get("TimeWatchConnectString", ""); } }

        /// <summary>
        /// 集群性能监控库连接
        /// </summary>
        public static string UnityLogConnectString { get { return ConfigHelper.Get("UnityLogConnectString", ""); } }

        /// <summary>
        /// 业务消息推送平台连接
        /// </summary>
        public static string NotifyPushConnectString { get { return ConfigHelper.Get("NotifyPushConnectString", ""); } }


        static BSFConfig()
        {
            //try
            //{
            //    ConfigInit();
            //}
            //catch { }
        }


       

       
        
    }
}
