using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core;
using TaskManager.Domain;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.Tools;
using BSF.BaseService.TaskManager;
using BSF.BaseService.TaskManager.SystemRuntime;
using BSF.Db;
using TaskManager.Core;


namespace TaskManager.Node.SystemRuntime
{
    /// <summary>
    /// 任务操作提供者
    /// 提供任务的开始，关闭,重启，卸载
    /// </summary>
    public class TaskProvider
    {
        /// <summary>
        /// 任务的开启
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Start(int taskid)
        {
            var taskruntimeinfo = TaskPoolManager.CreateInstance().Get(taskid.ToString());
            if (taskruntimeinfo != null)
            {
                throw new Exception("任务已在运行中");
            }

            taskruntimeinfo = new NodeTaskRuntimeInfo();
            taskruntimeinfo.TaskLock = new TaskLock();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
                {
                    tb_task_dal taskdal = new tb_task_dal();
                    taskruntimeinfo.TaskModel = taskdal.Get(c, taskid);
                    tb_version_dal versiondal = new tb_version_dal();
                    taskruntimeinfo.TaskVersionModel = versiondal.GetCurrentVersion(c, taskid, taskruntimeinfo.TaskModel.taskversion);
                });
            string filelocalcachepath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\"+GlobalConfig.TaskDllCompressFileCacheDir + @"\" + taskruntimeinfo.TaskModel.id + @"\" + taskruntimeinfo.TaskModel.taskversion + @"\" +
                taskruntimeinfo.TaskVersionModel.zipfilename;
            string fileinstallpath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\"+GlobalConfig.TaskDllDir + @"\" +  taskruntimeinfo.TaskModel.id ;
            string fileinstallmainclassdllpath = fileinstallpath + @"\" + taskruntimeinfo.TaskModel.taskmainclassdllfilename;
            string taskshareddlldir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + GlobalConfig.TaskSharedDllsDir;
             
            BSF.Tool.IOHelper.CreateDirectory(filelocalcachepath);
            BSF.Tool.IOHelper.CreateDirectory(fileinstallpath);
            System.IO.File.WriteAllBytes(filelocalcachepath, taskruntimeinfo.TaskVersionModel.zipfile);

            CompressHelper.UnCompress(filelocalcachepath, fileinstallpath);
            //拷贝共享程序集
            BSF.Tool.IOHelper.CopyDirectory(taskshareddlldir, fileinstallpath);
            try
            {
                new TaskAssemblyRedirect().TryRebulidDll(fileinstallmainclassdllpath, taskruntimeinfo.TaskModel.taskmainclassnamespace);
                var dlltask = new AppDomainLoader<BaseDllTask>().Load(fileinstallmainclassdllpath, taskruntimeinfo.TaskModel.taskmainclassnamespace, out taskruntimeinfo.Domain);
                var sdktaskmodel = new BSF.BaseService.TaskManager.Model.tb_task_model();
                PropertyHelper.Copy(taskruntimeinfo.TaskModel, sdktaskmodel);
                dlltask.SystemRuntimeInfo = new TaskSystemRuntimeInfo()
                {
                    TaskConnectString = GlobalConfig.TaskDataBaseConnectString,
                    TaskModel = sdktaskmodel
                };
                //加载AppConfig配置
                var appconfig  = new TaskAppConfigInfo();
                if (!string.IsNullOrEmpty(taskruntimeinfo.TaskModel.taskappconfigjson))
                {
                    appconfig = new BSF.Serialization.JsonProvider().Deserialize<TaskAppConfigInfo>(taskruntimeinfo.TaskModel.taskappconfigjson);
                }
                SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
                {
                    tb_config_dal configdal = new tb_config_dal();
                    var cs = configdal.GetList(c);
                    foreach (var o in cs)
                    {
                        if (!appconfig.ContainsKey(o.configkey))
                            appconfig.Add(o.configkey,o.configvalue); 
                    }
                });

                dlltask.AppConfig = appconfig;
                taskruntimeinfo.DllTask = dlltask;
                bool r = TaskPoolManager.CreateInstance().Add(taskid.ToString(), taskruntimeinfo);
                SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
                {
                    tb_task_dal taskdal = new tb_task_dal();
                    taskdal.UpdateTaskState(c, taskid, (int)EnumTaskState.Running);
                });
                LogHelper.AddTaskLog("节点开启任务成功", taskid);
                return r;
            }
            catch (Exception exp)
            {
                DisposeTask(taskid, taskruntimeinfo,true);
                throw exp;
            }
        }
        /// <summary>
        /// 任务的关闭
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Stop(int taskid)
        {
            var taskruntimeinfo = TaskPoolManager.CreateInstance().Get(taskid.ToString());
            if (taskruntimeinfo == null)
            {
                throw new Exception("任务不在运行中");
            }

            var r= DisposeTask(taskid, taskruntimeinfo,false);

            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
            {
                tb_task_dal taskdal = new tb_task_dal();
                taskdal.UpdateTaskState(c, taskid, (int)EnumTaskState.Stop);
            });
            LogHelper.AddTaskLog("节点关闭任务成功", taskid);
            return r;
        }
        /// <summary>
        /// 任务的卸载
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public bool Uninstall(int taskid)
        {
            var taskruntimeinfo = TaskPoolManager.CreateInstance().Get(taskid.ToString());
            if (taskruntimeinfo == null)
            {
                throw new Exception("任务不在运行中");
            }
            var r= DisposeTask(taskid, taskruntimeinfo,true);

            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
            {
                tb_task_dal taskdal = new tb_task_dal();
                taskdal.UpdateTaskState(c, taskid, (int)EnumTaskState.Stop);
            });
            LogHelper.AddTaskLog("节点卸载任务成功", taskid);
            return r;
        }
        /// <summary>
        /// 任务的资源释放
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="taskruntimeinfo"></param>
        /// <returns></returns>
        private bool DisposeTask(int taskid, NodeTaskRuntimeInfo taskruntimeinfo, bool isforceDispose)
        {
            if (taskruntimeinfo != null && taskruntimeinfo.DllTask != null)
                try { taskruntimeinfo.DllTask.Dispose(); taskruntimeinfo.DllTask = null; }
                catch (TaskSafeDisposeTimeOutException ex)
                {
                    LogHelper.AddNodeError("强制资源释放之任务资源释放", ex); 
                    if (isforceDispose == false)
                        throw ex;
                }
                catch (Exception e) { LogHelper.AddNodeError("强制资源释放之任务资源释放", e); }
            if (taskruntimeinfo != null && taskruntimeinfo.Domain != null)
                try { new AppDomainLoader<BaseDllTask>().UnLoad(taskruntimeinfo.Domain); taskruntimeinfo.Domain = null; }
                catch (Exception e) { LogHelper.AddNodeError("强制资源释放之应用程序域释放", e); }
            if (TaskPoolManager.CreateInstance().Get(taskid.ToString()) != null)
                try { TaskPoolManager.CreateInstance().Remove(taskid.ToString()); }
                catch (Exception e) { LogHelper.AddNodeError("强制资源释放之任务池释放", e); }
            LogHelper.AddTaskLog("节点已对任务进行资源释放", taskid);
            return true;
        }
    }
}
