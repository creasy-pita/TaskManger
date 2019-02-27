using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using BSF.Extensions;
using BSF.Tool;
using BSF.Config;
using BSF.BaseService.Monitor;
using Microsoft.AspNetCore.Http.Extensions;

namespace BSF.Log
{
    public class TimeWatchLogInfo
    {

        /// <summary>
        /// 耗时日志类型：普通日志=0，api接口日志=1，sql日志=2
        /// </summary>
        public EnumTimeWatchLogType logtype { get; set; }

        /// <summary>
        /// 日志标识,sql类型则为sql哈希 string.hash(),api类型则为url,普通日志则为方法名
        /// </summary>
        public int logtag { get; set; }

        /// <summary>
        /// 当前url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 当前信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 来源ip(代码执行ip)
        /// </summary>
        public string fromip { get; set; }

        /// <summary>
        /// sqlip地址
        /// </summary>
        public string sqlip { get; set; }

        /// <summary>
        /// 其他记录标记信息
        /// </summary>
        public string remark { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (true)
            {
                sb.Append("logtype:" + logtype.ToString() + ",");
            }
            if (true)
            {
                sb.Append("logtag:" + logtag.ToString() + ",");
            }
            if (!string.IsNullOrEmpty(url))
            {
                sb.Append("url:" + url.ToString() + ",");
            }
            if (!string.IsNullOrEmpty(msg))
            {
                sb.Append("msg:" + msg.ToString() + ",");
            }
            if (!string.IsNullOrEmpty(fromip))
            {
                sb.Append("fromip:" + fromip.ToString() + ",");
            }
            if (!string.IsNullOrEmpty(sqlip))
            {
                sb.Append("sqlip:" + sqlip.ToString() + ",");
            }
            if (!string.IsNullOrEmpty(remark))
            {
                sb.Append("remark:" + remark.ToString() + ",");
            }

            return sb.ToString().TrimEnd(',');
        }
    }
    /// <summary>
    /// 简易耗时打印 车毅
    /// </summary>
    public class TimeWatchLog
    {
        public static string FilePath = System.AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + "\\" + "timewatch.log";
        private static string CurrentIp { get; set; }
        public static bool IfWatch = false;
        public DateTime StartTime;
        static TimeWatchLog()
        {

        }
        public TimeWatchLog()
        {
            Start();
        }
        /// <summary>
        /// 开启
        /// </summary>
        public void Start()
        {
            StartTime = DateTime.Now;
        }

        public double Debug()
        {
            return (DateTime.Now - StartTime).TotalSeconds;
        }

        public static double Debug(Action action)
        {
            var startTime = DateTime.Now;
            action.Invoke();
            return (DateTime.Now - startTime).TotalSeconds;
        }

        /// <summary>
        /// 写普通耗时日志(url哈希为logtag)
        /// </summary>
        /// <param name="msg"></param>
        public void Write(string msg)
        {
            EnumTimeWatchLogType type = EnumTimeWatchLogType.Common;
            string url = "";
            try { url = ((System.Web.HttpContext.Current != null) ? (System.Web.HttpContext.Current.Request.GetDisplayUrl().SubString2(90)) : ""); } catch { }
            Write(new TimeWatchLogInfo()
            {
                fromip = "",
                logtype = type,
                logtag = url.GetHashCode(),
                url = url,
                msg = msg,
                remark = "",
                sqlip = "",
            });
        }

        /// <summary>
        /// 写普通耗时日志(方法名哈希为logtag)
        /// </summary>
        /// <param name="msg"></param>
        public void Write(string methodname, string msg)
        {
            EnumTimeWatchLogType type = EnumTimeWatchLogType.Common;
            Write(new TimeWatchLogInfo()
            {
                fromip = "",
                logtype = type,
                logtag = methodname.GetHashCode(),
                url = methodname.SubString2(90),
                msg = msg,
                remark = "",
                sqlip = "",
            });
        }


        public void Write(TimeWatchLogInfo timewatchloginfo)
        {
            try
            {
                if (!BSFConfig.IsWriteTimeWatchLog)
                    return;

                if (BSFConfig.IsWriteTimeWatchLogToLocalFile)
                {
                    string filepath = System.AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + "\\timewatchlog\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".time.log";
                    IOHelper.CreateDirectory(filepath);

                    System.IO.File.AppendAllText(filepath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + timewatchloginfo.ToString() + "-耗时: " + (DateTime.Now - StartTime).TotalSeconds + " s \r\n");
                }
                if (BSFConfig.IsWriteTimeWatchLogToMonitorPlatform)
                {
                    if (string.IsNullOrWhiteSpace(CurrentIp))
                    {
                        IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
                        IPAddress ipAddr = ipHost.AddressList[0];
                        CurrentIp = ipAddr.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(timewatchloginfo.fromip))
                    {
                        timewatchloginfo.fromip = CurrentIp;
                    }

                    if (timewatchloginfo.logtype == EnumTimeWatchLogType.ApiUrl)
                    {
                        BSF.BaseService.BaseServiceContext.MonitorProvider.AddTimeWatchApiLog(new BaseService.Monitor.Base.Entity.TimeWatchLogApiInfo()
                        {
                            fromip = timewatchloginfo.fromip.NullToEmpty(),
                            logcreatetime = DateTime.Now,
                            url = timewatchloginfo.url.SubString2(90).NullToEmpty(),
                            msg = timewatchloginfo.msg.SubString2(900).NullToEmpty(),
                            projectname = BSFConfig.ProjectName,
                            tag = timewatchloginfo.remark.NullToEmpty(),
                            time = (DateTime.Now - StartTime).TotalSeconds,
                        });
                    }
                    BSF.BaseService.BaseServiceContext.MonitorProvider.AddTimeWatchLog(new BaseService.Monitor.Base.Entity.TimeWatchLogInfo()
                    {
                        fromip = timewatchloginfo.fromip.NullToEmpty(),
                        logtype = (byte)timewatchloginfo.logtype,
                        logcreatetime = DateTime.Now,
                        logtag = timewatchloginfo.logtag,
                        url = timewatchloginfo.url.SubString2(90).NullToEmpty(),
                        msg = timewatchloginfo.msg.SubString2(900).NullToEmpty(),
                        projectname = BSFConfig.ProjectName.NullToEmpty(),
                        remark = timewatchloginfo.remark.SubString2(900).NullToEmpty(),
                        sqlip = timewatchloginfo.sqlip.NullToEmpty(),
                        time = (DateTime.Now - StartTime).TotalSeconds,
                    });

                }
            }
            catch { }

            //#if DEBUG
            //            string info = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + timewatchloginfo.msg + "-耗时: " + (DateTime.Now - StartTime).TotalSeconds + " s \r\n";
            //            Debug.WriteLine(info);
            //#else
            //#endif
        }

    }
}