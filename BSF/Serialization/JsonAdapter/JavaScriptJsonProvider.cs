using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

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
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(o);

        }


        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public override object Deserialize(string s, Type type)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize(s, type);

        }
    }
}
