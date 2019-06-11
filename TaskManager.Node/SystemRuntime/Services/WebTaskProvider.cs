using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using TaskManager.Core;
using TaskManager.Node.SystemRuntime.ProcessService;

namespace TaskManager.Node.SystemRuntime.Services
{
    public class WebTaskProvider
    {
        public delegate void StartDelegate(TomcatEventArgs args);
        public event StartDelegate OnStartingEvent;
        public event StartDelegate BeginStartEvent;
        public event StartDelegate StartCompeleteEvent;
        public event StartDelegate ServerRefuseEvent;

        public bool Start(TomcatEntity t)
        {
            try
            {
                //TomcatEventArgs args = new TomcatEventArgs
                //{
                //    Message = $"正在启动1 服务名：{t.TableName} ;服务端口{t.Port}"
                //};
                //BeginStartEvent?.Invoke(args);
                StartTomcat(t.Path, t.StartFileName, t.StartArguments);
                if (DoCheck(t))
                {
                    //TomcatEventArgs args1 = new TomcatEventArgs
                    //{
                    //    Message = $"启动完成 服务名：{t.TableName} ;服务端口{t.Port}"
                    //};
                    //StartCompeleteEvent?.Invoke(args1);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool DoCheck(TomcatEntity t)
        {
            bool flag = false;
            int maxCheckTimes = 100;
            int currTimes = 0;
            Stopwatch tw = new Stopwatch();
            while (!flag && currTimes <= maxCheckTimes)
            {
                System.Threading.Thread.Sleep(3000);
                currTimes++;
                HttpWebRequest request;
                HttpWebResponse response = null;
                try
                {
                    tw.Reset();
                    tw.Start();
                    request = (HttpWebRequest)HttpWebRequest.Create(t.HealthCheckUrl);
                    request.Timeout = 10000;
                    response = (HttpWebResponse)request.GetResponse();
                    tw.Stop();
                }
                catch (Exception ex)
                {
                    tw.Stop();
                    //TomcatEventArgs args = new TomcatEventArgs
                    //{
                    //    //Message = $"Server Port:{t.Port} the {currTimes}  check finds tomcat server not ready, wait for another check! info:{ex.Message}"
                    //    Message = $"{t.TableName} server refused, wait for another check in 3 seconds later! info:{ex.Message}"
                    //};
                    //OnStartingEvent?.Invoke(args);
                    //Console.WriteLine($"Server Port:{t.Port} the {currTimes}  check finds tomcat server not ready, wait for another check! info:{ex.Message}");
                    continue;
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //TomcatEventArgs args = new TomcatEventArgs
                    //{
                    //    // Message = $"Server Port:{t.TableName} the {currTimes}  check findstomcat server is ok ! time elapse:{tw.ElapsedMilliseconds}ms"
                    //    Message = $"{t.TableName} server is ok !"
                    //};
                    //StartCompeleteEvent?.Invoke(args);
                    //Console.WriteLine($"Server Port:{t.Port} the {currTimes}  check findstomcat server is ok ! time elapse:{tw.ElapsedMilliseconds}ms");
                    flag = true;
                }
                else
                {
                    //TomcatEventArgs args = new TomcatEventArgs
                    //{
                    //    //Message = $"Server Port:{t.Port} the {currTimes}  check finds tomcat server not ready, wait for another checktime elapse:{tw.ElapsedMilliseconds}ms"
                    //    Message = $"{t.TableName}  server not ready, wait for another check in 3 seconds later"
                    //};
                    //OnStartingEvent?.Invoke(args);
                    //Console.WriteLine($"Server Port:{t.Port} the {currTimes}  check finds tomcat server not ready, wait for another checktime elapse:{tw.ElapsedMilliseconds}ms");
                }
            }
            return true;
        }
        public static void StartTomcat(string WorkingDirectory, string StartFileName, string StartFileArg)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.WorkingDirectory = WorkingDirectory;//工作目录
            CmdProcess.StartInfo.FileName = StartFileName;      // 命令
            CmdProcess.StartInfo.Arguments = StartFileArg;      // 参数
            CmdProcess.EnableRaisingEvents = true;                      // 启用Exited事件
           // CmdProcess.StartInfo.CreateNoWindow = false;
            CmdProcess.Start();
        }

        public bool Stop(TomcatEntity t)
        {
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
            string processId = ps.GetProcessByPort(t.Port);
            if(!string.IsNullOrEmpty( processId))
            {
                var p = Process.GetProcessById(Convert.ToInt32(processId));
                p.Kill();
            }
            //调用进程终止后会又延时，以下采用重试判断的方式
            int RetryCount = 10;
            while (RetryCount-- > 0)
            {
                if (string.IsNullOrEmpty(ps.GetProcessByPort(t.Port)))
                {
                    RetryCount = 0;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            return true;
        }
    }




    public class TomcatEntity
    {
        public string TableName { get; set; }
        public string Port { get; set; }
        public string Desc { get; set; }
        public string Path { get; set; }
        /// <summary>
        /// 健康检查url
        /// </summary>
        public string HealthCheckUrl { get; set; }
        /// <summary>
        /// 启动优先级， 小的先启动
        /// </summary>
        public int StartPriority { get; set; }
        /// <summary>
        /// 启动tomcat 命令行脚本中的  exe名称(比如 java -jar xxx.jar 中  的java) 或者 bat脚本名,比如 start.bat run 中的 start.bat
        /// 脚本文件需要设置全路径, 如果没有设置环境变量的exe 也需要设置全路径
        /// </summary>
        public string StartFileName { get; set; }

        /// <summary>
        /// 启动服务的命令行脚本中的  参数(比如 java -jar xxx.jar 中  的-jar xxx.jar) 
        /// </summary>
        public string StartArguments { get; set; }
    }

    public class TomcatEventArgs
    {
        public string Message { get; set; }
    }

    public enum TomcatConfigState
    {
        Add,
        Modify,
        Delete
    }
}
