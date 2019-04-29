using System;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.Tools;
using BSF.Db;
using System.IO;
using System.Diagnostics;
using System.Threading;
using TaskManager.Node.SystemRuntime.ProcessService;

namespace TaskManager.Node.SystemRuntime
{
    /// <summary>
    /// linux任务程序操作提供者
    /// 提供任务的开始，关闭,重启，卸载
    /// </summary>
    public class LinuxTaskProvider:ITaskProvider
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
            return LinuxStart(task);
        }
        
        public bool LinuxStart(tb_task_model task)
        {
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Linux.ToString());
            //判断是否已经运行
            string pId = ps.GetProcessByName(task.taskmainclassdllfilename);
            if (!string.IsNullOrEmpty(pId))
            {
                throw new Exception("任务已在运行中");
            }
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                string rootPath = AppDomain.CurrentDomain.BaseDirectory + GlobalConfig.TaskDllDir+"/";
                BSF.Tool.IOHelper.CreateDirectory(rootPath);
                LogHelper.AddNodeLog($"rootPath:{rootPath}……");

                string path = $"{rootPath}/{task.taskname}/";
                //判断程序目录是否已经存在
                if (!Directory.Exists(path))
                {
                    tb_version_dal versionDAL = new tb_version_dal();
                    var version = versionDAL.GetVersionByTaskID(conn, task.id);
                    BSF.Tool.IOHelper.CreateDirectory(path);
                    LogHelper.AddNodeLog($"path:{path}……");
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
                //使用 shell 命令开启 任务程序
                //Thread.Sleep(5000);
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $@"{task.taskmainclassdllfilename}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    //RedirectStandardError = false,
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
                    //Console.WriteLine(Output.ReadLine());
                }
                ///线程睡眠2s，等到脚本执行完成
                Thread.Sleep(2000);
                EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
                EnumTaskState enumTaskState = EnumTaskState.Stop;
                if (!string.IsNullOrEmpty(ps.GetProcessByName(task.taskmainclassdllfilename)))
                {
                    enumTaskCommandState = EnumTaskCommandState.Success;
                    enumTaskState = EnumTaskState.Running;
                }
                ///更新服务状态
                task.tasklaststarttime = DateTime.Now;
                task.taskstate = (byte)enumTaskState;
                new tb_task_dal().UpdateTask(conn, task);
                if (enumTaskCommandState == EnumTaskCommandState.Success)
                {
                    LogHelper.AddNodeLog($"节点:{task.nodeid}成功执行任务:{task.id}……");
                }
            });
                return true;
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
            return LinuxStop(task);
        }

        public bool LinuxStop(tb_task_model task)
        {
            IProcessService ps = ProcessServiceFactory.CreateProcessService(EnumOSState.Linux.ToString());
            string pId = ps.GetProcessByName(task.taskmainclassdllfilename);
            if (string.IsNullOrEmpty(pId))
            {
                throw new Exception("任务不在运行中");
            }
            bool result = false;
            ps.ProcessKill(int.Parse(pId));
            EnumTaskCommandState enumTaskCommandState = EnumTaskCommandState.Error;
            EnumTaskState enumTaskState = EnumTaskState.Stop;
            if (string.IsNullOrEmpty(ps.GetProcessByName(task.taskmainclassdllfilename)))
            {
                enumTaskCommandState = EnumTaskCommandState.Success;
                enumTaskState = EnumTaskState.Stop;
                result = true;
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
        /// 任务的卸载
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Uninstall(int taskid)
        {
            return false;
        }
    }
}
