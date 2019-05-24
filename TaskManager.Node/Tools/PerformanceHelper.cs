using BSF.Db;
using BSF.Extensions;
using BSF.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.SystemRuntime;
using TaskManager.Node.SystemRuntime.ProcessService;

namespace TaskManager.Node.Tools
{
    /// <summary>
    /// TBD lastTimeDic 的数据是按照pid的方式维护的，可能随着进程关闭而过期引起 混乱使用的问题，需要
    /// </summary>
    public class PerformanceHelper
    {
        private static Dictionary<string,DateTime> lastTimeDic=new Dictionary<string, DateTime>();
        private static Dictionary<string, TimeSpan> lastTotalProcessorTimeDic = new Dictionary<string, TimeSpan>();
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="model"></param>
        public static (double CPU, long memory) GetPerformenceInfo(string serviceName)
        {
            try
            {
                IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
                string pIdStr = ps.GetProcessByName(serviceName);
                if (string.IsNullOrEmpty(pIdStr)) return (0,0);
                Process p = Process.GetProcessById(int.Parse(pIdStr));
                DateTime lastTime=new DateTime();
                TimeSpan lastTotalProcessorTime = new TimeSpan();

                if (lastTimeDic.ContainsKey(pIdStr))
                {
                    lastTime = lastTimeDic[pIdStr];
                    lastTotalProcessorTime = lastTotalProcessorTimeDic[pIdStr];
                }

                DateTime curTime ;
                TimeSpan curTotalProcessorTime ;
                double CPUUsage = 0; long memorySize;
                //cpu
                if (lastTime == null || lastTime == new DateTime())
                {
                    lastTime = DateTime.Now;
                    lastTotalProcessorTime = p.TotalProcessorTime;
                    lastTimeDic.Add(pIdStr, lastTime);
                    lastTotalProcessorTimeDic.Add(pIdStr, lastTotalProcessorTime);
                }
                else
                {
                    curTime = DateTime.Now;
                    curTotalProcessorTime = p.TotalProcessorTime;
                    CPUUsage = (curTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);
                    lastTime = curTime;
                    lastTotalProcessorTime = curTotalProcessorTime;
                    lastTimeDic[pIdStr] = curTime;
                    lastTotalProcessorTimeDic[pIdStr] = curTotalProcessorTime;
                }
                //memory
                memorySize = p.WorkingSet64;
                return ( CPUUsage,  memorySize);
            }
            catch (Exception exp)
            {
                return (0, 0);
            }
        }

        public static (double CPU, long memory) GetPerformenceInfoByPort(string port)
        {
            try
            {
                IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
                string pIdStr = ps.GetProcessByPort(port);
                if (string.IsNullOrEmpty(pIdStr)) return (0, 0);
                Process p = Process.GetProcessById(int.Parse(pIdStr));
                DateTime lastTime = new DateTime();
                TimeSpan lastTotalProcessorTime = new TimeSpan();

                if (lastTimeDic.ContainsKey(pIdStr))
                {
                    lastTime = lastTimeDic[pIdStr];
                    lastTotalProcessorTime = lastTotalProcessorTimeDic[pIdStr];
                }

                DateTime curTime;
                TimeSpan curTotalProcessorTime;
                double CPUUsage = 0; long memorySize;
                //cpu
                if (lastTime == null || lastTime == new DateTime())
                {
                    lastTime = DateTime.Now;
                    lastTotalProcessorTime = p.TotalProcessorTime;
                    lastTimeDic.Add(pIdStr, lastTime);
                    lastTotalProcessorTimeDic.Add(pIdStr, lastTotalProcessorTime);
                }
                else
                {
                    curTime = DateTime.Now;
                    curTotalProcessorTime = p.TotalProcessorTime;
                    CPUUsage = (curTotalProcessorTime.TotalMilliseconds - lastTotalProcessorTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / Convert.ToDouble(Environment.ProcessorCount);
                    lastTime = curTime;
                    lastTotalProcessorTime = curTotalProcessorTime;
                    lastTimeDic[pIdStr] = curTime;
                    lastTotalProcessorTimeDic[pIdStr] = curTotalProcessorTime;
                }
                //memory
                memorySize = p.WorkingSet64;
                return (CPUUsage, memorySize);
            }
            catch (Exception exp)
            {
                return (0, 0);
            }
        }

    }
}
