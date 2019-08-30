using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Util
{
    /// <summary>
    /// 类型转为工具类
    /// </summary>
    public class CastUtil
    {
        /// <summary>
        /// 判断字符串是否能转化为 int 类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CanCastInt(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    int.Parse(source);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

    }
}
