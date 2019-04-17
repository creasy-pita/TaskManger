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
    }
}
