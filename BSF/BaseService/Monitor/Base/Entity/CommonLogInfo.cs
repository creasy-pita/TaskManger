using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.Monitor.Base.Entity
{
    public class CommonLogInfo
    {
        /// <summary>
        /// 数据库创建时间
        /// </summary>
        public DateTime sqlservercreatetime { get; set; }

        /// <summary>
        /// 日志项目中创建时间
        /// </summary>
        public DateTime logcreatetime { get; set; }

        /// <summary>
        /// 日志类型:一般非正常错误,系统级严重错误,一般业务日志,系统日志
        /// </summary>
        public Byte logtype { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectname { get; set; }

        /// <summary>
        /// 日志唯一标示(简短的方法名或者url,便于归类)
        /// </summary>
        public string logtag { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string msg { get; set; }
    }
}
