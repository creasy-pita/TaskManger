using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.Monitor.Base.Entity
{
    public class TimeWatchLogInfo 
    {
        /// <summary>
        /// 数据库本地创建时间
        /// </summary>
        public DateTime sqlservercreatetime { get; set; }

        /// <summary>
        /// 日志创建时间
        /// </summary>
        public DateTime logcreatetime { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public double time { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectname { get; set; }

        /// <summary>
        /// 耗时日志类型：普通日志=0，api接口日志=1，sql日志=2
        /// </summary>
        public Byte logtype { get; set; }

        /// <summary>
        /// 日志标识,sql类型则为sql哈希,api类型则为url
        /// </summary>
        public int logtag { get; set; }

        /// <summary>
        /// 当前url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 当前信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 来源ip(代码执行ip)
        /// </summary>
        public string fromip { get; set; }

        /// <summary>
        /// sqlip地址
        /// </summary>
        public string sqlip { get; set; }

        /// <summary>
        /// 其他记录标记信息
        /// </summary>
        public string remark { get; set; }
    }
}
