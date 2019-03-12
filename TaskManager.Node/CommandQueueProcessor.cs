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
        public static int lastMaxID = -1;
        private static object _lockRunLoop = new object();
        public static void ServiceRunning()
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
        }
        static void RunCommond()
        {
            lock (_lockRunLoop)
            {
                try
                {
                    List<tb_command_model> commands = new List<tb_command_model>();
                    try
                    {
                        SqlHelper.ExcuteSql(GlobalConfig.ConnectionString, (conn) =>
                        {
                            tb_command_dal commanddal = new tb_command_dal();
                            if (lastMaxID < 0)
                            {
                                lastMaxID = commanddal.GetMaxCommandID(conn);
                            }
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
                        EnumTaskCommandState enumTaskCommandState=EnumTaskCommandState.Error;
                        EnumTaskState enumTaskState = EnumTaskState.UnInstall;
                        try
                        {
                            SqlHelper.ExcuteSql(GlobalConfig.ConnectionString, (conn) =>
                            {
                                tb_task_dal taskDAL = new tb_task_dal();
                                var task = taskDAL.GetOneTask(conn, c.taskid);

                                tb_version_dal versionDAL = new tb_version_dal();
                                var version = versionDAL.GetVersionByTaskID(conn, c.taskid);

                                string path = $"{AppDomain.CurrentDomain.BaseDirectory}{task.taskname}";
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
                        catch(Exception ex)
                        {
                            LogHelper.AddTaskError("任务执行错误", c.taskid, ex);
                        }
                    }
                }
                catch(Exception ex)
                {
                    LogHelper.AddNodeLog($"节点执行错误，错误原因:{ex.Message}");
                }
            }
        }
    }
}
