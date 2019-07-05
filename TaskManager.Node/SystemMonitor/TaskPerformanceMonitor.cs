using BSF.Db;
using BSF.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.Tools;


namespace TaskManager.Node.SystemMonitor
{
    /// <summary>
    /// 任务性能监控者
    /// 用于检测当前任务运行的性能情况，通知到数据库
    /// </summary>
    public class TaskPerformanceMonitor : BaseMonitor
    {
        public override int Interval
        {
            get
            {
                return 5000;
            }
        }
        protected override void Run()
        {
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
            {
                tb_task_dal dal = new tb_task_dal();
                List<tb_task_model> alltasks =  dal.GetTaskByNodeID(c,GlobalConfig.NodeID);
                IEnumerable<tb_task_model> tasks = alltasks.Where((t) => t.taskstate == 1);
                foreach (var task in tasks)
                {
                    string fileinstallpath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "/" + GlobalConfig.TaskDllDir + "/" +  task.taskname+"/";
                    double dirsizeM = -1;
                    if (System.IO.Directory.Exists(fileinstallpath))
                    {
                        long dirsize = TaskManager.Core.IOHelper.DirSize(new DirectoryInfo(fileinstallpath));
                        dirsizeM = (double)dirsize / 1024 / 1024;
                    }
                    (double cpu, long memorySize) = PerformanceHelper.GetPerformenceInfo(task.taskfindbatchscript);
                    tb_performance_dal nodedal = new tb_performance_dal();
                    nodedal.AddOrUpdate(c, new Domain.Model.tb_performance_model()
                    {
                        cpu = cpu,
                        memory = (double)memorySize / 1024 / 1024,
                        installdirsize = dirsizeM,
                        taskid = task.id,
                        lastupdatetime = DateTime.Now,
                        nodeid = GlobalConfig.NodeID
                    });
                }

            });

        }
    }
}
