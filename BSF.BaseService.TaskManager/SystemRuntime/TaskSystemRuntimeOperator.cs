using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSF.BaseService.TaskManager.Model;
using BSF.Serialization;
using BSF.Extensions;
using BSF.Db;

namespace BSF.BaseService.TaskManager.SystemRuntime
{
    /// <summary>
    /// 任务运行时底层操作类
    /// 仅平台内部使用
    /// </summary>
    public class TaskSystemRuntimeOperator
    {
        /// <summary>
        /// 任务dll实例引用
        /// </summary>
        protected BaseDllTask DllTask = null;
        protected string localtempdatafilename = "localtempdata.json.txt";

        public TaskSystemRuntimeOperator(BaseDllTask dlltask)
        {
            DllTask = dlltask;
        }

        public void SaveLocalTempData(object obj)
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + localtempdatafilename;
            var json = new JsonProvider().Serializer(obj);
            System.IO.File.WriteAllText(filename, json);
        }
        public T GetLocalTempData<T>() where T : class
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')+ "\\" + localtempdatafilename;
            if (!System.IO.File.Exists(filename))
                return null;
            string content = System.IO.File.ReadAllText(filename);
            var obj = new JsonProvider().Deserialize<T>(content);
            return obj;
        }
        public void SaveDataBaseTempData(object obj)
        {
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_tempdata_dal taskdal = new Dal.tb_tempdata_dal();
                taskdal.SaveTempData(c, DllTask.SystemRuntimeInfo.TaskModel.id, new JsonProvider().Serializer(obj));
            });
        }
        public T GetDataBaseTempData<T>() where T:class
        {
            string json = null;
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_tempdata_dal taskdal = new Dal.tb_tempdata_dal();
                json = taskdal.GetTempData(c, DllTask.SystemRuntimeInfo.TaskModel.id);
            });
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            var obj = new JsonProvider().Deserialize<T>(json);
            return obj;
        }

        public void UpdateLastStartTime(DateTime time)
        {
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_task_dal taskdal = new Dal.tb_task_dal();
                taskdal.UpdateLastStartTime(c, DllTask.SystemRuntimeInfo.TaskModel.id,time);
            });
        }

        public void UpdateLastEndTime(DateTime time)
        {
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_task_dal taskdal = new Dal.tb_task_dal();
                taskdal.UpdateLastEndTime(c, DllTask.SystemRuntimeInfo.TaskModel.id, time);
            });
        }

        public void UpdateTaskError(DateTime time)
        {
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_task_dal taskdal = new Dal.tb_task_dal();
                taskdal.UpdateTaskError(c, DllTask.SystemRuntimeInfo.TaskModel.id, time);
            });
        }

        public void UpdateTaskSuccess()
        {
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_task_dal taskdal = new Dal.tb_task_dal();
                taskdal.UpdateTaskSuccess(c, DllTask.SystemRuntimeInfo.TaskModel.id);
            });
        }

        public void AddLog(tb_log_model model)
        {
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_log_dal logdal = new Dal.tb_log_dal();
                model.msg = model.msg.SubString2(1000);
                logdal.Add(c, model);
            });
        }

        public void AddError(tb_error_model model)
        {
            AddLog(new tb_log_model { logcreatetime=model.errorcreatetime, logtype=model.errortype, msg=model.msg, taskid=model.taskid });
            SqlHelper.ExcuteSql(DllTask.SystemRuntimeInfo.TaskConnectString, (c) =>
            {
                Dal.tb_error_dal errordal = new Dal.tb_error_dal();
                model.msg = model.msg.SubString2(1000);
                errordal.Add(c, model);
            });
        }
    }
}
