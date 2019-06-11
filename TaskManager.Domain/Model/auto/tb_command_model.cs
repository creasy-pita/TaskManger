using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_command Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_command_model
    {
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        /// <summary>
        /// 命令json
        /// </summary>
        public string command { get; set; }
        
        /// <summary>
        /// 命令名，参考代码枚举
        /// </summary>
        public string commandname { get; set; }
        
        /// <summary>
        /// 命令执行状态，参考代码枚举
        /// </summary>
        public Byte commandstate { get; set; }
        
        /// <summary>
        /// 任务id
        /// </summary>
        public int taskid { get; set; }
        
        /// <summary>
        /// 节点id
        /// </summary>
        public int nodeid { get; set; }

        /// <summary>
        /// 任务类型  0 控制台服务程序任务  1 web服务程序任务
        /// </summary>
        public int tasktype { get; set; }

        /// <summary>
        /// 命令创建时间
        /// </summary>
        public DateTime commandcreatetime { get; set; }
        
    }
}