using BSF.Db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.Commands;
using TaskManager.Node.SystemRuntime;
using TaskManager.Node.Tools;

namespace TaskManager.Node
{
    public class CheckState
    {
        public static void CheckRunning()
        {
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                tb_task_dal taskDAL = new tb_task_dal();
                var tasks = taskDAL.GetTaskByNodeID(conn, GlobalConfig.NodeID);
                foreach(tb_task_model task in tasks)
                {
                    if (task.taskstate == 1 && ServiceHelper.ServiceState(task.taskname)!= EnumTaskState.Running)
                    {                      
                        try
                        {
                            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{task.taskname}";
                            if (!Directory.Exists(path))
                            {
                                throw new Exception($"目录{path}不存在");
                            }
                            CommandFactory.Execute(path + "\\start.bat");
                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (ServiceHelper.ServiceState(task.taskname) != EnumTaskState.Running)
                            {
                                ///服务停止开启失败，发送邮件提醒                               
                            }
                        }
                        catch(Exception e)
                        {
                            LogHelper.AddTaskError("服务节点无法开启", task.id, e);
                        }
                    }
                    ///如果当前服务运行状态和数据库服务状态不一致，更新数据库服务状态
                    if (task.taskstate != (int)ServiceHelper.ServiceState(task.taskname))
                    {
                        int state = (int)ServiceHelper.ServiceState(task.taskname);
                        if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.UnInstall)
                        {
                            state = 0;
                        }
                        taskDAL.UpdateTaskState(conn, task.id, state);
                    }
                }
            });
        }
    }
}
