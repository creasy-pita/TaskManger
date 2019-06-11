using BSF.Api;
using System;
using System.Threading;
using TaskManager.Core;

namespace TaskManager.Node
{
    class Program
    {
        static int period = 20;
        static string identityName = "ZS.Service.Manager";
        static void Main(string[] args)
        {
            NodeMain_Load();
            Thread t2 = new Thread(Monitor);
            t2.Start();
            Console.ReadKey();
        }


        static void Monitor()
        {
            //TBD 解决TimeJob 中修改 任务状态 会与 WebTaskProvider 修改任务状态 不同步的问题后 取消下行注释
            //ScheduleJob.ExecuteInterval<TimeJob>(period, identityName);
        }

        public static void NodeMain_Load()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GlobalConfig.TaskDataBaseConnectString) || GlobalConfig.NodeID <= 0)
                {
                    string url = GlobalConfig.TaskManagerWebUrl.TrimEnd('/') + "/OpenApi/" + "GetNodeConfigInfo/";
                    var r = ApiHelper.Post<dynamic>(url, new
                    {

                    });
                    if (r.success == false)
                    {
                        throw new Exception("请求" + url + "失败,请检查配置中“任务调度平台站点url”配置项");
                    }

                    var appconfiginfo = r.data;
                    string connectstring = appconfiginfo.taskDataBaseConnectString;
                    appconfiginfo.taskDataBaseConnectString = StringDESHelper.DecryptDES(connectstring, "dyd88888888");

                    //var appconfiginfo = new NodeAppConfigInfo();
                    //appconfiginfo.TaskDataBaseConnectString = "server=192.168.17.201;Initial Catalog=dyd_bs_task;User ID=sa;Password=Xx~!@#;";
                    //appconfiginfo.NodeID = 1;

                    if (string.IsNullOrWhiteSpace(GlobalConfig.TaskDataBaseConnectString))
                        GlobalConfig.TaskDataBaseConnectString = appconfiginfo.taskDataBaseConnectString;
                    if (GlobalConfig.NodeID <= 0)
                        GlobalConfig.NodeID = appconfiginfo.nodeID;
                }

                //BSF.Tool.IOHelper.CreateDirectory(GlobalConfig.TaskSharedDllsDir + @"\");
                CommandQueueProcessor.Run();
                //注册后台监控
                //GlobalConfig.Monitors.Add(new SystemMonitor.TaskRecoverMonitor());
                GlobalConfig.Monitors.Add(new SystemMonitor.TaskPerformanceMonitor());
                GlobalConfig.Monitors.Add(new SystemMonitor.NodeHeartBeatMonitor());
                //GlobalConfig.Monitors.Add(new SystemMonitor.TaskStopMonitor());
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
