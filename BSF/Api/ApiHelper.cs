using BSF.Log;
using BSF.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BSF.Extensions;
using System.Collections;

namespace BSF.Api
{
    /// <summary>
    /// api帮助类
    /// </summary>
    public class ApiHelper
    {

        public static ClientResult<T> Post<T>(string url, Dictionary<string, Object> param)
        {
            return HttpPost<T>(url, param);
        }
        /// <summary>
        /// 获取api结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// 
        public static ClientResult<T> Post<T>(string url, Object param)
        {

            return HttpPost<T>(url, GetDictionaryOfParam(param));
        }

        public static ClientResult<T> PostWithSign<T>(string url, string appsecret, Object param)
        {
            var dataDic = GetDictionaryOfParam(param);
            var dic = new Dictionary<string, string>();
            foreach (var p in param.GetType().GetProperties())
            {
                var value = p.GetValue(param, null);
                dic.Add(p.Name, (value!=null||value is string)? value.ToString() :null);
            }
            string sign = new SignProvider().CreateSign(dic, appsecret);
            dataDic.Add(SignProvider.signKey, sign);

            return HttpPost<T>(url, GetDictionaryOfParam(dataDic));
        }


        private static Dictionary<string, object> GetDictionaryOfParam(Object param)
        {
            var dic = new Dictionary<string,object>();
            foreach (var p in param.GetType().GetProperties())
            {
                dic.Add(p.Name, p.GetValue(param, null));
            }
            return dic;
        }


        private static ClientResult<T> HttpPost<T>(string url, Dictionary<string, object> param)
        {
            return HttpPostBase<ClientResult<T>>(url, param, (r) =>
            {
                ClientResult<T> clientResult = null;
                try
                {
                    clientResult = new JsonProvider().Deserialize<ClientResult<T>>(r);
                }
                catch (Exception exp)
                {
                    try
                    {
                        //clientResult返回错误时,非标准类型的data类型返回的自动修正为默认值;
                        //标准的返回错误：data应该为返回类型的默认值。
                        var result = new JsonProvider().Deserialize<ClientResult<Object>>(r);
                        if (result.success == false)
                        {
                            result.data = default(T);
                            clientResult = new JsonProvider().Deserialize<ClientResult<T>>(new JsonProvider().Serializer(result));
                        }
                        else
                            throw exp;
                    }
                    catch { throw exp; }
                }
                clientResult.responsetext = r;
                return clientResult;
            });
        }

        private static T HttpPostBase<T>(string url, Dictionary<string, object> param, Func<string, T> action)
        {
            //可以记录api调用的网络耗时，便于优化以及一些错误日志的记录拦截
            //兼容后续版本的api通信机制
            try
            {
                //TimeWatchLog watch = new TimeWatchLog();//网络耗时打印
                string msg = "";
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    msg = HttpContext.Current.Request.RawUrl.ToString();

                string clientResultText = new HttpProvider().PostWithJson(url, param);
                T r = action.Invoke(clientResultText);
                //watch.Write(new TimeWatchLogInfo() { url = msg, logtag = msg.GetHashCode(), msg = msg, logtype =  BaseService.Monitor.EnumTimeWatchLogType.ApiUrl});
                return r;
            }
            catch (Exception exp)
            {
                string message = string.Format("api调用出错,【url】:{0}\r\n", url.NullToEmpty());
                ErrorLog.Write(message, exp);
                throw new Exception(message + exp.Message);

            }
        }

    }
}
