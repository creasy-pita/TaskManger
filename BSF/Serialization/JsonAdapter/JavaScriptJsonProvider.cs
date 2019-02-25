using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Web.Script.Serialization;

namespace BSF.Serialization.JsonAdapter
{
    /// <summary>
    /// JavaScriptSerializer 方式Json序列化
    /// System.Web.Script.Serialization
    /// </summary>
    public class JavaScriptJsonProvider : BaseJsonProvider
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override string Serializer(object o)
        {
            return JsonConvert.SerializeObject(o);
        }


        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public override object Deserialize(string s, Type type)
        {
            return JsonConvert.DeserializeObject(s, type);
        }
    }
}
