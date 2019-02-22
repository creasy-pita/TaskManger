using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.Tool
{
    public class MD5Helper
    {
        /// <summary>
        /// MD5加密 32位小写
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string En32MD5(string text)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(text);
            byte[] hs = md5.ComputeHash(bs);
            string str = "";
            for (int i = 0; i < hs.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                str += Convert.ToString(hs[i], 16).PadLeft(2, '0');

            }
            return str.PadLeft(32, '0').ToLower();
        }

        
    }
}
