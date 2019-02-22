using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.BaseService.Monitor.Base.Entity
{
    public class TimeWatchLogApiInfo 
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime sqlservercreatetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime logcreatetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string projectname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string fromip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string tag { get; set; }
    }
}
