using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Domain.Model
{

    /// <summary>
    /// tb_error Data Structure.
    /// </summary>
    [Serializable]
    public partial class tb_config_model
    {
        /*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string configkey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string configvalue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime lastupdatetime { get; set; }

    }
}
