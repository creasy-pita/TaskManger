using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TaskManager.Domain.Model
{
    /// <summary>
    /// tb_version Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_package_model
    {
	/*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/
        
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        
        
        /// <summary>
        /// 程序包名称
        /// </summary>
        public string packagename { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime createtime { get; set; }
        
        
    }
}