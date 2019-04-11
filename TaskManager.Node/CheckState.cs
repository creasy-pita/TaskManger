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
                var taskNode = (new tb_node_dal()).GetOneNode(conn, GlobalConfig.NodeID);
                var tasks = taskDAL.GetTaskByNodeID(conn, GlobalConfig.NodeID);
                foreach(tb_task_model task in tasks)
                {
                    string processId = string.Empty;
                    if (taskNode.nodeostype == "0")
                    {
                        processId = ProcessHelper.GetWindowsProcess(task.taskmainclassdllfilename);
                    }
                    else
                    {
                        processId = ProcessHelper.GetProcess(task.taskmainclassdllfilename);
                    }
                    if (task.taskstate == 1 && string.IsNullOrEmpty( processId))
                    {
                        try
                        {
                            tb_command_model c = new tb_command_model()
                            {
                                command = "",
                                commandcreatetime = DateTime.Now,
                                commandname = EnumTaskCommandName.StartTask.ToString() ,
                                taskid = task.id,
                                nodeid = task.nodeid,
                                commandstate = (int)EnumTaskCommandState.None
                            };
                            CommandFactory.Execute(c);
                        }
                        catch(Exception e)
                        {
                            LogHelper.AddTaskError("服务节点无法开启", task.id, e);
                        }
                    }
                    //再次匹配状态 如果当前服务运行状态和数据库服务状态不一致，更新数据库服务状态
                    processId = string.Empty;
                    if (taskNode.nodeostype == "0")
                    {
                        processId = ProcessHelper.GetWindowsProcess(task.taskmainclassdllfilename);
                    }
                    else
                    {
                        processId = ProcessHelper.GetProcess(task.taskmainclassdllfilename);
                    }
                    int state = string.IsNullOrEmpty(processId)?0:1;
                    if (task.taskstate != state)
                    {
                        taskDAL.UpdateTaskState(conn, task.id, state);
                    }
                }
            });
        }

    }
}
