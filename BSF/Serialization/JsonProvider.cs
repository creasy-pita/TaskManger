using BSF.Serialization.JsonAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BSF.Serialization
{
    /// <summary>
    /// jason 序列化方式
    /// </summary>
    public class JsonProvider: BaseJsonProvider
    {
        private EnumJsonMode jsonMode;
        public JsonProvider(EnumJsonMode mode = EnumJsonMode.JavaScriptBussiness)
        {
            jsonMode = mode;
        }

        private BaseJsonProvider CreateJsonProvider()
        {
            if (jsonMode == EnumJsonMode.JavaScript)
            {
                return new JavaScriptJsonProvider();
            }
            if (jsonMode == EnumJsonMode.JavaScriptBussiness)
            {
                return new JavaScriptBussinessJsonProvider();
            }
            else if (jsonMode == EnumJsonMode.Newtonsoft)
            {
                return new NewtonsoftJsonProvider();
            }
            else if (jsonMode == EnumJsonMode.DataContract)
            {
                return new DataContractJsonProvider();
            }
            throw new BSF.Base.BSFException("未找到相应的json序列化Provider");
        }

        /// <summary>
        /// jason序列化
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override string Serializer(object o)
        {
            try
            { 
                return CreateJsonProvider().Serializer(o);
            }
            catch (Exception exp)
            {
                throw new BSF.Base.BSFException(string.Format("json序列化出错,jsonMode:{1},序列化类型：{0}", o.GetType().FullName,jsonMode.ToString()), exp);
            }
        }

        /// <summary>
        /// jason反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public T Deserialize<T>(string s)
        {
            try
            {
                return (T)CreateJsonProvider().Deserialize(s,typeof(T));
            }
            catch (Exception exp)
            {
                throw new BSF.Base.BSFException(string.Format("json反序列化出错,jsonMode:{1},内容：{0}", s,jsonMode.ToString()), exp);
            }
        }
        /// <summary>
        /// jason反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public object Deserialize(string s, Type type)
        {
            try
            {
                return CreateJsonProvider().Deserialize(s,type);
            }
            catch (Exception exp)
            {
                throw new BSF.Base.BSFException(string.Format("json反序列化出错,jsonMode:{1},内容：{0}", s,jsonMode.ToString()), exp);
            }
        }
    }
}
