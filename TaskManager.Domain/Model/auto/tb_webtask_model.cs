using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_webtask Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_webtask_model
    {
        
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
        /// 
        /// </summary>
        public Byte taskstate { get; set; }
        
        
        /// <summary>
        /// 
        /// </summary>
        public int taskport { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string taskhealthcheckurl { get; set; }
        
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public string taskstopfilename { get; set; }

        /// <summary>
        /// 关闭服务的命令行脚本中的  参数(比如 java -jar xxx.jar 中  的-jar xxx.jar)
        /// </summary>
        public string taskstoparguments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string taskremark { get; set; }
        
    }
}