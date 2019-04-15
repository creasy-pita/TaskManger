using BSF.Db;
using BSF.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TaskManager.Core;
using TaskManager.Core.Redis;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.Commands;
using TaskManager.Node.SystemRuntime;
using TaskManager.Node.Tools;

namespace TaskManager.Node
{

    public class CommandQueueProcessor
    {
        private static object _lockRunLoop = new object();

        private static System.Threading.Thread thread;
        /// <summary>
        /// 上一次日志扫描的最大id
        /// </summary>
        public static int lastMaxID = -1;
        static CommandQueueProcessor()
        {
            thread = new System.Threading.Thread(Running);
            thread.IsBackground = true;
            thread.Start();
        }
        /// <summary>
        /// 运行处理循环
        /// </summary>
        public static void Run()
        { }

        static void Running()
        {
            RedisHelper.RedisListner((channel, msg) =>
            {
                try
                {
                    //msg = "{'CommondType':2,'nodeid':1}";
                    RedisCommondInfo redisCommondInfo = null;
                    redisCommondInfo = new BSF.Serialization.JsonProvider().Deserialize<RedisCommondInfo>(msg);
                    if (redisCommondInfo != null)
                    {
                        if (redisCommondInfo.CommondType == EnumCommondType.TaskCommand && redisCommondInfo.NodeId == GlobalConfig.NodeID)
                        {
                            RunCommond();
                        }
                        else if (redisCommondInfo.CommondType == EnumCommondType.ConfigUpdate)
                        {
                            RedisHelper.RefreashRedisServerIP();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.AddNodeError($"Redis获取订阅消息出错,{ex.Message}", ex);
                }
            }, (info) =>
               {
                   if (info != null)
                   {
                       LogHelper.AddNodeError("Redis订阅出错," + info.Message.NullToEmpty(), info.Exception);
                   }
               });
            RuningCommandLoop();
        }

        /// <summary>
        /// 运行消息循环
        /// </summary>
        static void RuningCommandLoop()
        {
            LogHelper.AddNodeLog("准备接受命令并运行消息循环...");
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                RunCommond();
                System.Threading.Thread.Sleep(5000);
            }
        }
        static void RunCommond()
        {
            lock (_lockRunLoop)
            {
                try
                {
                    List<tb_command_model> commands = new List<tb_command_model>();
                    tb_node_model node = new tb_node_model();
                    try
                    {
                        SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                        {
                            tb_command_dal commanddal = new tb_command_dal();
                            if (lastMaxID < 0)
                            {
                                lastMaxID = commanddal.GetMaxCommandID(conn);
                            }
                            node = new tb_node_dal().Get(conn, GlobalConfig.NodeID);
                            commands = commanddal.GetNodeCommands(conn, GlobalConfig.NodeID, lastMaxID);
                        });
                    }
                    catch(Exception ex)
                    {
                        LogHelper.AddNodeError("获取当前节点命令集错误", ex);
                    }
                    if (commands.Count > 0)
                    {
                        LogHelper.AddNodeLog("当前节点扫描到" + commands.Count + "条命令,并执行中....");
                    }
                    foreach (var c in commands)
                    {
                        try
                        {
                            CommandFactory.Execute(c);
                            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                            {
                                new tb_command_dal().UpdateCommandState(conn, c.id, (int)EnumTaskCommandState.Success);
                            });
                            LogHelper.AddNodeLog(string.Format("当前节点执行命令成功! id:{0},命令名:{1},命令内容:{2}", c.id, c.commandname, c.command));
                        }
                        catch (Exception exp1)
                        {
                            try
                            {
                                SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                                {
                                    new tb_command_dal().UpdateCommandState(conn, c.id, (int)EnumTaskCommandState.Error);
                                });
                            }
                            catch { }
                            LogHelper.AddTaskError("执行节点命令失败", c.taskid, exp1);
                        }
                        lastMaxID = Math.Max(lastMaxID, c.id);
                    }

                    //if (Enum.Parse(typeof(EnumOSState), node.nodeostype) as EnumOSState == EnumOSState.Linux)
                    //if (node.nodeostype =="1")
                    //    LinuxRun(commands);
                    //else
                    //    WindowsRun(commands);
                }
                catch(Exception ex)
                {
                    LogHelper.AddNodeLog($"节点执行错误，错误原因:{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 恢复已开启的任务
        /// </summary>
        static void RecoveryStartTasks()
        {
            try
            {
                LogHelper.AddNodeLog("当前节点启动成功,准备恢复已经开启的任务...");
                List<int> taskids = new List<int>();
                tb_node_model node = null;
                SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
                {
                    node = new tb_node_dal().Get(c, GlobalConfig.NodeID);
                    tb_task_dal taskdal = new tb_task_dal();
                    taskids = taskdal.GetTaskIDsByState(c, (int)EnumTaskState.Running, GlobalConfig.NodeID);
                });
                List<tb_command_model> commands = new List<tb_command_model>();

                foreach (var taskid in taskids)
                {
                    try
                    {
                        tb_command_model command = new tb_command_model()
                        {
                            command = "",
                            commandcreatetime = DateTime.Now,
                            commandname = EnumTaskCommandName.StartTask.ToString(),
                            commandstate = (int)EnumTaskCommandState.None,
                            nodeid = GlobalConfig.NodeID,
                            taskid = taskid,
                            id = -1
                        };
                        commands.Add(command);

                        //CommandFactory.Execute(new tb_command_model()
                        //{
                        //    command = "",
                        //    commandcreatetime = DateTime.Now,
                        //    commandname = EnumTaskCommandName.StartTask.ToString(),
                        //    commandstate = (int)EnumTaskCommandState.None,
                        //    nodeid = GlobalConfig.NodeID,
                        //    taskid = taskid,
                        //    id = -1
                        //});
                    }
                    catch (Exception exp)
                    {
                        LogHelper.AddTaskError(string.Format("恢复已经开启的任务{0}失败", taskid), taskid, exp);
                    }
                }
                if (node.nodeostype == EnumOSState.Linux.Tostring())
                    LinuxRun(commands);
                else
                    WindowsRun(commands);
                LogHelper.AddNodeLog(string.Format("恢复已经开启的任务完毕，共{0}条任务重启", taskids.Count));
            }
            catch (Exception exp)
            {
                LogHelper.AddNodeError("恢复已经开启的任务失败", exp);
            }
        }

        static void LinuxRun(List<tb_command_model> commands)
        {
            foreach (var c in commands)
            {
                EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
                EnumTaskState enumTaskState = EnumTaskState.UnInstall;
                try
                {
                    SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                    {
                        tb_task_dal taskDAL = new tb_task_dal();
                        var task = taskDAL.GetOneTask(conn, c.taskid);

                        tb_version_dal versionDAL = new tb_version_dal();
                        var version = versionDAL.GetVersionByTaskID(conn, c.taskid);
                        string rootPath = AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir;
                        BSF.Tool.IOHelper.CreateDirectory(rootPath);
                        string path = $"{AppDomain.CurrentDomain.BaseDirectory}{GlobalConfig.TaskDllDir}/{task.taskname}";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                            if (version != null)
                            {
                                string zipFilePath = $"{path}/{version.zipfilename}";
                                ///数据库二进制转压缩文件
                                CompressHelper.ConvertToFile(version.zipfile, zipFilePath);
                                CompressHelper.UnCompress(zipFilePath, path);
                                ///删除压缩文件
                                File.Delete(zipFilePath);
                            }
                            else
                            {
                                throw new Exception($"在tb_version表中未查到taskid:{c.taskid}数据");
                            }
                        }
                        if (c.commandname == EnumTaskCommandName.StartTask.Tostring())
                        {
                            string pId = ProcessHelper.GetProcess(task.taskmainclassdllfilename);
                            if (!string.IsNullOrEmpty(pId))
                            {
                                //关闭 之前的进程
                                ProcessHelper.KillProcess(pId);
                            }
                            ////线程睡眠5s，等待服务安装完成
                            //Thread.Sleep(5000);
                            CommandFactory.LinuxExecute(rootPath, task.taskname, task.taskmainclassdllfilename);

                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (!string.IsNullOrEmpty(ProcessHelper.GetProcess(task.taskmainclassdllfilename) ))
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Running;
                            }
                        }
                        else if (c.commandname == EnumTaskCommandName.ReStartTask.Tostring())
                        {
                            string pId = ProcessHelper.GetProcess(task.taskmainclassdllfilename);
                            if (!string.IsNullOrEmpty(pId))
                            {
                                //关闭 之前的进程
                                ProcessHelper.KillProcess(pId);
                            }
                            ///线程睡眠5s，等待服务安装完成
                            Thread.Sleep(5000);

                            CommandFactory.LinuxExecute(rootPath, task.taskname, task.taskmainclassdllfilename);

                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (!string.IsNullOrEmpty(ProcessHelper.GetProcess(task.taskmainclassdllfilename)))
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Running;
                            }
                        }
                        else if (c.commandname == EnumTaskCommandName.StopTask.Tostring())
                        {
                            string pId = ProcessHelper.GetProcess(task.taskmainclassdllfilename);
                            if (!string.IsNullOrEmpty(pId))
                            {
                                //关闭 之前的进程
                                ProcessHelper.KillProcess(pId);
                            }
                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (string.IsNullOrEmpty(ProcessHelper.GetProcess(task.taskmainclassdllfilename)))
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Stop;
                            }
                        }
                        else if (c.commandname == EnumTaskCommandName.UninstallTask.Tostring())
                        {
                            string pId = ProcessHelper.GetProcess(task.taskmainclassdllfilename);
                            if (!string.IsNullOrEmpty(pId))
                            {
                                //关闭 之前的进程
                                ProcessHelper.KillProcess(pId);
                            }
                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (string.IsNullOrEmpty(ProcessHelper.GetProcess(task.taskmainclassdllfilename)))
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Stop;
                            }
                        }
                        ///更新命令状态
                        new tb_command_dal().UpdateCommandState(conn, c.id, (int)enumTaskCommandState);
                        ///更新服务状态
                        new tb_task_dal().UpdateTaskState(conn, c.taskid, (int)enumTaskState);

                        if (enumTaskCommandState == EnumTaskCommandState.Success)
                        {
                            LogHelper.AddNodeLog($"节点:{c.nodeid}成功执行任务:{c.taskid}……");
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.AddTaskError("任务执行错误", c.taskid, ex);
                }
            }
        }

        static void WindowsRun(List<tb_command_model> commands)
        {
            foreach (var c in commands)
            {
                EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
                EnumTaskState enumTaskState = EnumTaskState.UnInstall;
                try
                {
                    SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                    {
                        tb_task_dal taskDAL = new tb_task_dal();
                        var task = taskDAL.GetOneTask(conn, c.taskid);

                        tb_version_dal versionDAL = new tb_version_dal();
                        var version = versionDAL.GetVersionByTaskID(conn, c.taskid);

                        BSF.Tool.IOHelper.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}{GlobalConfig.TaskDllDir}");
                        string path = $"{AppDomain.CurrentDomain.BaseDirectory}{GlobalConfig.TaskDllDir}\\{task.taskname}";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                            if (version != null)
                            {
                                string zipFilePath = $"{path}\\{version.zipfilename}";
                                ///数据库二进制转压缩文件
                                CompressHelper.ConvertToFile(version.zipfile, zipFilePath);
                                CompressHelper.UnCompress(zipFilePath, path);
                                ///删除压缩文件
                                File.Delete(zipFilePath);
                                ///初始化shell脚本
                                InitScript.InstallScript(task.taskname, task.taskmainclassdllfilename);

                                InitScript.UninstallScript(task.taskname);

                                InitScript.StartScript(task.taskname);

                                InitScript.StopScript(task.taskname);
                            }
                            else
                            {
                                throw new Exception($"在tb_version表中未查到taskid:{c.taskid}数据");
                            }
                        }


                        if (c.commandname == EnumTaskCommandName.StartTask.Tostring())
                        {

                            if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.UnInstall)
                            {
                                CommandFactory.Execute(path + "\\install.bat");
                            }

                            ///线程睡眠5s，等待服务安装完成
                            Thread.Sleep(5000);
                            CommandFactory.Execute(path + "\\start.bat");

                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.Running)
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Running;
                            }
                        }
                        else if (c.commandname == EnumTaskCommandName.ReStartTask.Tostring())
                        {
                            CommandFactory.Execute(path + "\\start.bat");
                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.Running)
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Running;
                            }
                        }
                        else if (c.commandname == EnumTaskCommandName.StopTask.Tostring())
                        {
                            CommandFactory.Execute(path + "\\stop.bat");

                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.Stop)
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                                enumTaskState = EnumTaskState.Stop;
                            }
                        }
                        else if (c.commandname == EnumTaskCommandName.UninstallTask.Tostring())
                        {
                            if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.Running)
                            {
                                CommandFactory.Execute(path + "\\stop.bat");
                                Thread.Sleep(2000);
                            }
                            CommandFactory.Execute(path + "\\uninstall.bat");

                            ///线程睡眠2s，等到脚本执行完成
                            Thread.Sleep(2000);
                            if (ServiceHelper.ServiceState(task.taskname) == EnumTaskState.UnInstall)
                            {
                                enumTaskCommandState = EnumTaskCommandState.Success;
                            }
                        }
                        ///更新命令状态
                        new tb_command_dal().UpdateCommandState(conn, c.id, (int)enumTaskCommandState);
                        ///更新服务状态
                        new tb_task_dal().UpdateTaskState(conn, c.taskid, (int)enumTaskState);

                        if (enumTaskCommandState == EnumTaskCommandState.Success)
                        {
                            LogHelper.AddNodeLog($"节点:{c.nodeid}成功执行任务:{c.taskid}……");
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogHelper.AddTaskError("任务执行错误", c.taskid, ex);
                }
            }
        }
    }
}
