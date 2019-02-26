using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.Extensions
{
    /// <summary>
    /// Request(System.Web.HttpRequestBase)扩展类
    /// </summary>
    public static class RequestMethodHelper
    {
        /// <summary>
        /// 请求参数Get,Post
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        //public static List<string> RequestParams(this System.Web.HttpRequestBase Request)
        //{
        //    var rs = new List<string>();
        //    rs.AddRange(Request.Form.Keys);
        //    rs.AddRange(Request.Query.Keys);
        //    return rs.Distinct().ToList();
        //}

        /// <summary>
        /// 请求参数Get,Post
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static List<string> RequestParams(this HttpRequest Request)
        {
            var rs = new List<string>();
            if(Request.HasFormContentType)
                rs.AddRange(Request.Form.Keys);
            rs.AddRange(Request.Query.Keys);
            return rs.Distinct().ToList();
        }
        /// <summary>
        /// 获取请求参数值
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        //public static object RequestParamValue(this System.Web.HttpRequestBase Request,string key)
        //{
        //    if (Request.Form.Keys.Contains(key))
        //        return Request.Form[key];
        //    if(Request.Query.Keys.Contains(key))
        //        return Request.QueryString[key];
        //    return null;
        //}

        /// <summary>
        /// 获取请求参数值
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object RequestParamValue(this HttpRequest Request, string key)
        {
            if (Request.Form !=null && Request.Form.Keys.Contains(key))
                return Request.Form[key];
            if (Request.Query.Keys.Contains(key))
                return Request.Query[key];
            return null;
        }
    }
}
