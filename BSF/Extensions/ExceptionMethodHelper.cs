using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSF.Extensions;

namespace System
{
   public static class ExceptionMethodHelper
    {
        /// <summary>
        /// 获取详细错误堆栈信息
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        public static string DetailMessage(this Exception exp)
        {
            var expt = exp; string message = "";
            while (expt != null)
            {
                message += "→" + expt.Message.NullToEmpty()+"\r\n";
                expt = expt.InnerException;
            }
            return message;
        }
    }
}
