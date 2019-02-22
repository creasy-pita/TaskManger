using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Node.SystemRuntime;
using TaskManager.Node.Tools;
using BSF.Log;
using BSF.Db;
using TaskManager.Core;


namespace TaskManager.Node.SystemMonitor
{
    /// <summary>
    /// 任务的回收监控者
    /// 用于任务异常卸载的资源回收
    /// </summary>
    public class TaskRecoverMonitor : BaseMonitor
    {
        public override int Interval
        {
            get
            {
                return 1000 * 60;//1分钟扫描
            }
        }

        private List<int> lastscantaskids = new List<int>();

        protected override void Run()
        {

            List<int> taskids = new List<int>();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
                {
                    tb_task_dal taskdal = new tb_task_dal();
                    taskids = taskdal.GetTaskIDsByState(c, (int)EnumTaskState.Stop, GlobalConfig.NodeID);
                });
            List<int> currentscantaskids = new List<int>();
            foreach (var taskid in taskids)
            {

                var taskruntimeinfo = TaskPoolManager.CreateInstance().Get(taskid.ToString());
                if (taskruntimeinfo != null)
                {
                    currentscantaskids.Add(taskid);
                }

            }

            var recovertaskids = (from o in lastscantaskids
                                  from c in currentscantaskids
                                  where o == c
                                  select o).ToList();
            if (recovertaskids != null && recovertaskids.Count > 0)
                recovertaskids.ForEach((c) =>
                {
                    LogHelper.AddTaskError("任务资源运行异常,可能需要手动卸载", c, new Exception("任务处于停止状态，但是相应集群节点中，发现任务存在在运行池中未释放"));
                });
            lastscantaskids = currentscantaskids;
        }
    }
}
