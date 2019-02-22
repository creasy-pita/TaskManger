using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Node.Tools;
using BSF.Api;


namespace TaskManager.Node.SystemMonitor
{
    public class PingTaskWebMonitor : BaseMonitor
    {
        public override int Interval
        {
            get
            {
                return 1000*60;
            }
        }
        protected override void Run()
        {
            try
            {
                string url = GlobalConfig.TaskManagerWebUrl.TrimEnd('/') + "/OpenApi/" + "Ping/";
                var r = ApiHelper.Post<dynamic>(url, new
                {

                });
                if (r.success == false)
                {
                    LogHelper.AddNodeError("检测到任务平台站点异常", new Exception("节点任务平台Web保持心跳连接时出错"));
                }
            }
            catch(Exception exp){
                LogHelper.AddNodeError("检测到任务平台站点异常", exp);
            }

        }
    }
}
