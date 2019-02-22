using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSF.Serialization.JsonAdapter
{
    /// <summary>
    /// Json 序列化基础类库
    /// </summary>
    public abstract class BaseJsonProvider
    {
        /// <summary>
        /// jason序列化
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public virtual string Serializer(object o)
        {
            return null;
        }

        /// <summary>
        /// jason反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual object Deserialize(string s, Type type)
        {
            return null;
        }
    }
    /// <summary>
    /// Json序列化方式枚举
    /// </summary>
    public enum EnumJsonMode
    {
        /// <summary>
        /// System.Web.Script.Serialization （推荐使用）
        /// </summary>
        JavaScript,
        /// <summary>
        /// System.Web.Script.Serialization (常规业务json,解决问题如datetime序列化格式问题等)
        /// </summary>
        JavaScriptBussiness,
        /// <summary>
        /// System.Runtime.Serialization.Json
        /// </summary>
        DataContract,
        /// <summary>
        /// Newtonsoft.Json.dll
        /// </summary>
        Newtonsoft,

    }
}
