using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.Db
{
    public class SqlHelper
    {

        public static DateTime DefaultTime()
        {
            return new DateTime(1970,01,01);
        }

        public static void ExcuteSql(string connectstring, Action<DbConn> action)
        {
            ExcuteSql(connectstring, false, action);
        }

        public static void ExcuteSql(string connectstring, bool iswatchtimelog, Action<DbConn> action)
        {
            using (DbConn PubConn = DbConn.CreateConn(connectstring))
            {
                PubConn.Open();
                PubConn.IsWatchTime = iswatchtimelog;
                action(PubConn);
            }
        }

        /// <summary>
        /// sql访问拦截器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Visit<T>(Func<SimpleProcedureParameter, T> action)
        {
            try
            {
                var dicparams = new SimpleProcedureParameter();
                var r = action.Invoke(dicparams);
                return r;
            }
            catch (Exception exp)
            {
                //ErrorLog.Write("dal层sql调用出错:", exp);
                throw exp;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="par"></param>
        /// <param name="splits"></param>
        /// <returns></returns>
        public static string CmdInFromListForSimplePar<T>(SimpleProcedureParameter ps, List<T> splits, string inparam)
        {
            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (var s in splits)
            {
                string param = string.Format("@{0}{1}", inparam, index);
                sb.AppendFormat(param + ",");
                ps.Add(param, s);
                index++;
            }
            return sb.ToString().Trim(',');
        }

        /// <summary>
        /// ,分隔的in 
        /// 举例 name in ('a','b','c'); 
        /// </summary>
        /// <param name="splits"></param>
        /// <returns></returns>
        public static string CmdIn(List<Db.ProcedureParameter> par, string splits)
        {
            string[] ss = splits.Trim(',').Split(',');
            string r = "";
            int index = 0;
            foreach (var s in ss)
            {
                string param = string.Format("@inparam{0}", index);
                r += param + ",";
                par.Add(new Db.ProcedureParameter(param, s));
                index++;
            }
            return r.Trim(',');
        }

        /// <summary>
        /// ,分隔的in 
        /// 举例 name in ('a','b','c'); 
        /// </summary>
        /// <param name="splits"></param>
        /// <returns></returns>
        public static string CmdIn<T>(List<Db.ProcedureParameter> par, List<T> splits)
        {

            string r = "";
            int index = 0;
            foreach (var s in splits)
            {
                string param = string.Format("@inparam{0}", index);
                r += param + ",";
                par.Add(new Db.ProcedureParameter(param, s));
                index++;
            }
            return r.Trim(',');
        }
    }
}
