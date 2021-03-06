﻿using System;
using TaskManager.Core;
using TaskManager.Domain;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.Tools;
using BSF.Db;
using System.IO;
using System.Diagnostics;
using System.Threading;
using TaskManager.Node.SystemRuntime.ProcessService;
using TaskManager.Node.Exceptions;
using System.Collections.Generic;

namespace TaskManager.Node.SystemRuntime
{
    /// <summary>
    /// windows任务程序操作提供者
    /// 提供任务的开始，关闭,重启，卸载
    /// </summary>
    public class WindowsTaskProvider:ITaskProvider
    {
        /// <summary>
        /// 任务的开启
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Start(int taskid)
        {
            tb_task_model task = new tb_task_model();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                tb_task_dal taskDAL = new tb_task_dal();
                task = taskDAL.GetOneTask(conn, taskid);
            });
            return WindowsStart(task);
        }

        public bool WindowsStartOld(tb_task_model task)
        {
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
            //判断是否已经运行
            string pId = ps.GetProcessByName(task.taskmainclassdllfilename);
            if (!string.IsNullOrEmpty(pId))
            {
                throw new Exception("任务已在运行中");
            }
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                //TBD 修改为 AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir 
                //TBD 后期加入每个任务可定制部署目录
                // string rootPath = AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir + "\\";
                //string rootPath = "F:\\" + GlobalConfig.TaskDllDir + "\\";
                //BSF.Tool.IOHelper.CreateDirectory(rootPath);
                //string path = $"{rootPath}{task.taskname}\\";
                string path = task.taskpath;
                //判断程序目录是否已经存在
                if (!Directory.Exists(path))
                {
                    //下载程序集
                    tb_version_dal versionDAL = new tb_version_dal();
                    var version = versionDAL.GetVersionByTaskID(conn, task.id);
                    BSF.Tool.IOHelper.CreateDirectory(path);
                    if (version != null)
                    {
                        string zipFilePath = $"{path}{version.zipfilename}";
                        ///数据库二进制转压缩文件
                        CompressHelper.ConvertToFile(version.zipfile, zipFilePath);
                        CompressHelper.UnCompress(zipFilePath, path);
                        ///删除压缩文件
                        File.Delete(zipFilePath);
                    }
                    else
                    {
                        throw new Exception($"在tb_version表中未查到taskid:{task.id}数据");
                    }
                }

                #region//加载外部配置文件
                tb_task_config_dal task_Config_Dal = new tb_task_config_dal();
                List<tb_task_config_model> task_Config_Models = task_Config_Dal.GetList(conn, task.id);
                try
                {
                    foreach (var task_config_Model in task_Config_Models)
                    {
                        string fullPath = path + task_config_Model.relativePath + task_config_Model.filename;
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                        using (FileStream fs = File.Create(fullPath))
                        {
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(task_config_Model.filecontent);
                            fs.Write(buffer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"任务在加载外部配置文件出错:{ex.Message}");
                }

                #endregion

                //使用 shell 命令开启 任务程序
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $@"{task.taskmainclassdllfilename}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = $"{path}",
                };
                var ProcessStart = Process.Start(psi);
                if (ProcessStart == null)
                {
                    throw new Exception("无法执行进程");
                }
                else
                {
                    var Output = ProcessStart.StandardOutput;
                }

                EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
                EnumTaskState enumTaskState = EnumTaskState.Stop;
                int RetryCount = 10;
                while (RetryCount-- > 0)
                {
                    if (!string.IsNullOrEmpty(ps.GetProcessByName(task.taskmainclassdllfilename)))
                    {
                        enumTaskCommandState = EnumTaskCommandState.Success;
                        enumTaskState = EnumTaskState.Running;
                        RetryCount = 0;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }

                ///更新服务状态 服务启动时间
                //new tb_task_dal().UpdateTaskState(conn, task.id, (int)enumTaskState);
                task.tasklaststarttime = DateTime.Now;
                task.taskstate = (byte)enumTaskState;
                //if(conn.GetConnection().State != System.Data.ConnectionState.Open)
                //    LogHelper.AddNodeLog($"节点:{task.nodeid}执行任务:{task.id} new tb_task_dal().UpdateTask conn state {conn.GetConnection().State.ToString()}");
                new tb_task_dal().UpdateTask(conn, task);
                if (enumTaskCommandState == EnumTaskCommandState.Success)
                {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}成功执行任务:{task.id}……");
                }
            });

            /*
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, async (conn) =>
            {
                //TBD 修改为 AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir 
                //TBD 后期加入每个任务可定制部署目录
                // string rootPath = AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir + "\\";
                string rootPath ="F:\\" + GlobalConfig.TaskDllDir + "\\";
                BSF.Tool.IOHelper.CreateDirectory(rootPath);

                string path = $"{rootPath}{task.taskname}\\";
                //判断程序目录是否已经存在
                if (!Directory.Exists(path))
                {
                    //下载程序集
                    tb_version_dal versionDAL = new tb_version_dal();
                    var version = versionDAL.GetVersionByTaskID(conn, task.id);
                    BSF.Tool.IOHelper.CreateDirectory(path);
                    if (version != null)
                    {
                        string zipFilePath = $"{path}{version.zipfilename}";
                        ///数据库二进制转压缩文件
                        CompressHelper.ConvertToFile(version.zipfile, zipFilePath);
                        CompressHelper.UnCompress(zipFilePath, path);
                        ///删除压缩文件
                        File.Delete(zipFilePath);
                    }
                    else
                    {
                        throw new Exception($"在tb_version表中未查到taskid:{task.id}数据");
                    }
                }

                #region//加载外部配置文件
                tb_task_config_dal task_Config_Dal = new tb_task_config_dal();
                List<tb_task_config_model> task_Config_Models = task_Config_Dal.GetList(conn, task.id);
                try
                {
                    foreach (var task_config_Model in task_Config_Models)
                    {
                        string fullPath = path + task_config_Model.relativePath + task_config_Model.filename;
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                        using (FileStream fs = File.Create(fullPath))
                        {
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(task_config_Model.filecontent);
                            await fs.WriteAsync(buffer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"任务在加载外部配置文件出错:{ex.Message}");
                }

                #endregion

                //使用 shell 命令开启 任务程序
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $@"{task.taskmainclassdllfilename}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = $"{path}",
                };
                var ProcessStart = Process.Start(psi);
                if (ProcessStart == null)
                {
                    throw new Exception("无法执行进程");
                }
                else
                {
                    var Output = ProcessStart.StandardOutput;
                }

                EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
                EnumTaskState enumTaskState = EnumTaskState.Stop;
                int RetryCount = 10;
                while (RetryCount-- > 0)
                {
                    if (!string.IsNullOrEmpty(ps.GetProcessByName(task.taskmainclassdllfilename)))
                    {
                        enumTaskCommandState = EnumTaskCommandState.Success;
                        enumTaskState = EnumTaskState.Running;
                        RetryCount = 0;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }

                ///更新服务状态 服务启动时间
                //new tb_task_dal().UpdateTaskState(conn, task.id, (int)enumTaskState);
                task.tasklaststarttime = DateTime.Now;
                task.taskstate = (byte)enumTaskState;
                //if(conn.GetConnection().State != System.Data.ConnectionState.Open)
                //    LogHelper.AddNodeLog($"节点:{task.nodeid}执行任务:{task.id} new tb_task_dal().UpdateTask conn state {conn.GetConnection().State.ToString()}");
                new tb_task_dal().UpdateTask(conn, task);
                if (enumTaskCommandState == EnumTaskCommandState.Success)
                {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}成功执行任务:{task.id}……");
                }
            });
            */
            return true;
        }

        public string GetProcessId(tb_task_model task, IProcessService ps)
        {
            string pId = string.Empty;
            if (!string.IsNullOrEmpty(task.taskfindbatchscript))
            {
                pId = ps.GetProcessIdByBatchScript(task.taskfindbatchscript);
            }
            else
            {
                pId = ps.GetProcessByCondition(task.taskstartfilename, task.taskarguments);
            }
            return pId;
        }

        public bool WindowsStart(tb_task_model task)
        {
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
            //判断是否已经运行
            //string pId = ps.GetProcessIdByBatchScript(task.taskfindbatchscript);
            LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时检查之前是否在运行开始");

            string pId = GetProcessId(task, ps);
            LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时检查之前是否在运行结束");

            if (!string.IsNullOrEmpty(pId))
            {
                throw new Exception("任务已在运行中");
            }
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                //TBD 修改为 AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir 
                //TBD 后期加入每个任务可定制部署目录
                // string rootPath = AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir + "\\";
                //string rootPath = "F:\\" + GlobalConfig.TaskDllDir + "\\";
                //BSF.Tool.IOHelper.CreateDirectory(rootPath);

                //string path = $"{rootPath}{task.taskname}\\";
                string path = task.taskpath;
                //判断程序目录是否已经存在
                //TBD 对于路径的处理  要注意 每一段路径是否需要加 【\】
                if (!Directory.Exists(path))
                {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时开始程序集下载");
                    tb_packageversion_dal packageversion_Dal = new tb_packageversion_dal();
                    tb_packageversion_model packageversion_Model = packageversion_Dal.Get(conn, task.taskpackageversionid);
                    BSF.Tool.IOHelper.CreateDirectory(path);
                    if (packageversion_Model != null)
                    {
                        string zipFilePath = $"{path}{packageversion_Model.zipfilename}";
                        ///数据库二进制转压缩文件
                        CompressHelper.ConvertToFile(packageversion_Model.zipfile, zipFilePath);
                        CompressHelper.UnCompress(zipFilePath, path);
                        ///删除压缩文件
                        File.Delete(zipFilePath);
                    }
                    else
                    {
                        throw new Exception($"在tb_packageversion表中未查到taskid:{task.id}数据");
                    }
                    LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时完成程序集下载");

                }

                #region//加载外部配置文件
                tb_task_config_dal task_Config_Dal = new tb_task_config_dal();
                List<tb_task_config_model> task_Config_Models = task_Config_Dal.GetList(conn, task.id);
                try
                {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时开始外部配置文件加载");
                    foreach (var task_config_Model in task_Config_Models)
                    {
                        string fullPath = path + task_config_Model.relativePath + task_config_Model.filename;
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                        using (FileStream fs = File.Create(fullPath))
                        {
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(task_config_Model.filecontent);
                            fs.Write(buffer);
                        }
                    }
                    LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时完成外部配置文件加载");
                }
                catch (Exception ex)
                {
                    throw new Exception($"任务在加载外部配置文件出错:{ex.Message}");
                }

                #endregion

                LogHelper.AddNodeLog($"节点:{task.nodeid}开启任务{task.id}时打开程序进程...");
                //使用 shell 命令开启 任务程序
                var psi = new ProcessStartInfo
                {
                    FileName = task.taskstartfilename,
                    Arguments = $@"{task.taskarguments}",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WorkingDirectory = task.taskpath
                };
                var ProcessStart = new Process();
                ProcessStart.StartInfo = psi;
                //ProcessStart.EnableRaisingEvents=true;
                if (!ProcessStart.Start())
                {
                    throw new Exception("无法执行进程");
                }

                EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
                EnumTaskState enumTaskState = EnumTaskState.Stop;
                int RetryCount = 10;
                while (RetryCount-- > 0)
                {
                    //if (!string.IsNullOrEmpty(ps.GetProcessIdByBatchScript(task.taskfindbatchscript)))
                    if (!string.IsNullOrEmpty(GetProcessId(task, ps)))
                    {
                        enumTaskCommandState = EnumTaskCommandState.Success;
                        enumTaskState = EnumTaskState.Running;
                        RetryCount = 0;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }

                ///更新服务状态 服务启动时间
                //new tb_task_dal().UpdateTaskState(conn, task.id, (int)enumTaskState);
                task.tasklaststarttime = DateTime.Now;
                task.taskstate = (byte)enumTaskState;
                //if(conn.GetConnection().State != System.Data.ConnectionState.Open)
                //    LogHelper.AddNodeLog($"节点:{task.nodeid}执行任务:{task.id} new tb_task_dal().UpdateTask conn state {conn.GetConnection().State.ToString()}");
                new tb_task_dal().UpdateTask(conn, task);
                if (enumTaskCommandState == EnumTaskCommandState.Success)
                {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}成功执行任务:{task.id}……");
                }
                else {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}成功执行失败:{task.id}……");
                }
            });

            return true;
        }

        public bool WindowsStop(tb_task_model task)
        {
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Windows.ToString());
            //string pId = ps.GetProcessIdByBatchScript(task.taskfindbatchscript);
            string pId = GetProcessId(task,ps);
            if (string.IsNullOrEmpty(pId))
            {
                throw new TaskAlreadyNotRunningException("任务不在运行中");
            }
            bool result = false;
            ps.ProcessKill(int.Parse(pId));
            EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
            EnumTaskState enumTaskState = EnumTaskState.Stop;
            //调用进程终止后会又延时，以下采用重试判断的方式
            int RetryCount = 10;
            while (RetryCount-- > 0)
            {
                //if (string.IsNullOrEmpty(ps.GetProcessIdByBatchScript(task.taskfindbatchscript)))
                if (string.IsNullOrEmpty(GetProcessId(task, ps)))
                {
                    enumTaskCommandState = EnumTaskCommandState.Success;
                    enumTaskState = EnumTaskState.Stop;
                    result = true;
                    RetryCount = 0;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                tb_task_dal taskdal = new tb_task_dal();
                task.tasklastendtime = DateTime.Now;
                task.taskstate = (byte)enumTaskState;
                taskdal.UpdateTask(conn, task);
            });
            LogHelper.AddTaskLog("节点关闭任务成功", task.id);
            return result;
        }

        /// <summary>
        /// 任务的关闭
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Stop(int taskid)
        {
            tb_task_model task = new tb_task_model();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                tb_task_dal taskDAL = new tb_task_dal();
                task = taskDAL.GetOneTask(conn, taskid);
            });
            return WindowsStop(task);
        }
        /// <summary>
        /// 任务的卸载
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Uninstall(int taskid)
        {
            tb_task_model task = new tb_task_model();
            try
            {
                bool re = false;
                try
                {
                    re = Stop(taskid);
                }
                catch (TaskAlreadyNotRunningException)//捕获此类异常不做处理
                {
                    re = true;
                }
                if (re)
                {
                    SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
                    {
                        tb_task_dal taskDAL = new tb_task_dal();
                        task = taskDAL.GetOneTask(conn, taskid);
                    });
                    //TBD 检查任务是否已经停止运行

                    #region //如果有卸载脚本， 则运行 卸载
                    if (!string.IsNullOrEmpty(task.taskuninstallbatchscript))
                    {
                        //使用 shell 命令开启 任务程序
                        var psi = new ProcessStartInfo
                        {
                            FileName = "cmd",
                            Arguments = $@"{task.taskuninstallbatchscript}",
                            UseShellExecute = true,
                            CreateNoWindow = false,
                            WorkingDirectory = task.taskpath
                        };
                        var ProcessStart = new Process();
                        ProcessStart.StartInfo = psi;
                        if (!ProcessStart.Start())
                        {
                            throw new Exception("无法正确执行卸载任务的脚本");
                        }
                    }
                    #endregion

                    Thread.Sleep(10000);
                    //TBD 修改为 AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir 
                    //TBD 后期加入每个任务可定制部署目录
                    // string rootPath = AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir + "\\";
                    //string rootPath = "F:\\" + GlobalConfig.TaskDllDir + "\\";
                    //string path = $"{rootPath}{task.taskname}\\";

                    string path = task.taskpath;

                    if (Directory.Exists(path))
                    {
                        //TBD 考虑后期 先备份这个目录中需要备份的文件，或者提前以挂载形式挂载到外部目录
                        Directory.Delete(path,true);
                    }
                    LogHelper.AddTaskLog("节点卸载任务成功", task.id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.AddTaskError($"节点:{task.nodeid}执行卸载任务失败", taskid, ex);
                return false;
            }
        }
    }
}
