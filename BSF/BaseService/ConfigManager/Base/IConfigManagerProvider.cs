using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.BaseService.ConfigManager
{
    /// <summary>
    /// 定义配置中心操作接口
    /// </summary>
    public interface IConfigManagerProvider
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configkey">配置的key（配置项）</param>
        /// <returns></returns>
        T Get<T>(string configkey);

         /// <summary>
        /// 判断配置是否存在配置
        /// </summary>
        /// <param name="configkey">配置的key（配置项）</param>
        /// <returns></returns>
         bool TryGet<T>(string configkey, out string value);
    }
}
