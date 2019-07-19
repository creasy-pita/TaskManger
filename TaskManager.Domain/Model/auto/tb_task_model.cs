using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_task Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_task_model
    {
	/*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskname { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int categoryid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int nodeid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime taskcreatetime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime taskupdatetime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime tasklaststarttime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime tasklastendtime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime tasklasterrortime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskerrorcount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskruncount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskcreateuserid { get; set; }
        
        /// <summary>
        /// 0 停止  1 运行中
        /// </summary>
        public Byte taskstate { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int taskversion { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskappconfigjson { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskcron { get; set; }

        /// <summary>
        /// 废弃不使用 obsolete discard
        /// </summary>
        public string taskmainclassnamespace { get; set; }

        /// <summary>
        /// 废弃不使用 obsolete discard
        /// </summary>
        public string taskmainclassdllfilename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string taskremark { get; set; }

        /// <summary>
        /// 程序根目录 rootpath
        /// </summary>
        public string taskpath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string taskstartfilename { get; set; }

        /// <summary>
        /// 开启服务的命令行脚本中的  参数(比如 java -jar xxx.jar 中  的-jar xxx.jar)
        /// </summary>
        public string taskarguments { get; set; }

        /// <summary>
        /// 查找此任务进程id的批处理脚本，一般运行在powershell或者cmd 目前支持powershell的脚本
        /// </summary>
        public string taskfindbatchscript { get; set; }

        /// <summary>
        /// 卸载程序包前需要执行的脚本，比如 用于 解除window服务的注册的脚本
        /// </summary>
        public string taskuninstallbatchscript { get; set; }

    }
}