using BSF.BaseService.TaskManager.Dal;
using BSF.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.SystemRuntime;
using TaskManager.Node.SystemRuntime.ProcessService;
using TaskManager.Node.SystemRuntime.Services;
using TaskManager.Node.Tools;

namespace TaskManager.Node.Commands
{
    /// <summary>
    /// 开启任务命令
    /// </summary>
    public class StartWebTaskCommand : BaseCommand
    {
        public override void Execute()
        {
            tb_webtask_model webtask = new tb_webtask_model();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                webtask = new tb_webtask_dal().Get(conn, this.CommandInfo.taskid);
            });
            TomcatEntity t = new TomcatEntity {
                Path = webtask.taskpath,
                Port = webtask.taskport.ToString(),
                HealthCheckUrl = webtask.taskhealthcheckurl,
                StartFileName = webtask.taskstartfilename,
                StartArguments = webtask.taskarguments
            };
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
            //判断是否已经运行
            string pId = ps.GetProcessByPort(webtask.taskport.ToString());
            if (!string.IsNullOrEmpty(pId))
            {
                throw new Exception("任务已在运行中");
            }
            WebTaskProvider webTaskProvider = new WebTaskProvider();
            if (webTaskProvider.Start(t))
            {
                SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                {
                    ///更新服务状态 服务启动时间
                    //new tb_task_dal().UpdateTaskState(conn, task.id, (int)enumTaskState);
                    webtask.tasklaststarttime = DateTime.Now;
                    webtask.taskstate = (byte)EnumTaskState.Running;
                    new tb_webtask_dal().UpdateTask(conn, webtask);
                    LogHelper.AddNodeLog($"节点:{webtask.nodeid}成功执行任务:{webtask.id}……");
                });
            }
        }
    }
}
