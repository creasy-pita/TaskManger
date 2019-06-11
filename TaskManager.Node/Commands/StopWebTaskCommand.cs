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
using TaskManager.Node.SystemRuntime.Services;
using TaskManager.Node.Tools;

namespace TaskManager.Node.Commands
{
    /// <summary>
    /// 关闭任务命令
    /// </summary>
    public class StopWebTaskCommand:BaseCommand
    {
        public override void Execute()
        {
            tb_webtask_model webtask = new tb_webtask_model();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                webtask = new tb_webtask_dal().Get(conn, this.CommandInfo.taskid);
            });
            TomcatEntity t = new TomcatEntity
            {
                Port = webtask.taskport.ToString(),
            };
            WebTaskProvider webTaskProvider = new WebTaskProvider();
            webTaskProvider.Stop(t);
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                tb_webtask_dal taskdal = new tb_webtask_dal();
                webtask.tasklastendtime = DateTime.Now;
                webtask.taskstate = (byte)EnumTaskState.Stop;
                taskdal.UpdateTask(conn, webtask);
            });
            LogHelper.AddTaskLog("节点关闭web任务成功", webtask.id);
        }
    }
}
