using BSF.BaseService.ConfigManager;
using BSF.BaseService.Monitor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSF.BaseService
{
    /// <summary>
    /// 基础服务上下文
    /// </summary>
    public static class BaseServiceContext
    {
        /// <summary>
        /// 注册配置中心接口
        /// </summary>
        public static IConfigManagerProvider ConfigManagerProvider = null;
        /// <summary>
        /// 注册监控平台接口
        /// </summary>
        public static IMonitorProvider MonitorProvider = null;

        static BaseServiceContext()
        {
            /*自己实现IOC 依赖注入*/

            ConfigManagerProvider = TryToRegisterContext<IConfigManagerProvider>("BSF.BaseService.ConfigManager.ConfigManagerProvider",
            new[] { "BSF.Full", "BSF.BaseService.ConfigManager" });

            MonitorProvider = TryToRegisterContext<IMonitorProvider>("BSF.BaseService.Monitor.MonitorProvider",
            new[] { "BSF.Full", "BSF.BaseService.Monitor" });
        }

        private static T TryToLoadContext<T>(string typeFullName, out bool isSuccess) where T : class
        {
            isSuccess = false;
            try
            {
                Type type = null;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName.StartsWith("BSF", StringComparison.CurrentCultureIgnoreCase))
                    {
                        type = assembly.GetType(typeFullName);
                        if (type != null)
                        {
                            var onew = Activator.CreateInstance(type);
                            if (onew is T)
                            {
                                T t = (T)onew;
                                if (t != null)
                                    isSuccess = true;
                                return t;
                            }
                        }
                            
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static T TryToRegisterContext<T>(string typeFullName, string[] tryDllNames) where T : class
        {
            //默认注册上下文
            bool isLoadConfigManagerProvider = false;
            T configManagerProvider = null;
            configManagerProvider = TryToLoadContext<T>(typeFullName, out isLoadConfigManagerProvider);

            foreach (var dllName in tryDllNames)
            {
                if (isLoadConfigManagerProvider == true && configManagerProvider != null)
                    break;
                if (TryToLoadDll(dllName) == true)
                {
                    configManagerProvider = TryToLoadContext<T>(typeFullName, out isLoadConfigManagerProvider);
                }
            }
            return configManagerProvider;
        }

        private static bool TryToLoadDll(string dllName)
        {
            var isSuccess = false;
            try
            {
                //string dllPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\" + dllName + ".dll";
                //if (System.IO.File.Exists(dllPath) == true)
                //{
                    Assembly.Load(dllName);
                //}
                isSuccess = true;
            }
            catch (Exception ex)
            {
            }
            return isSuccess;
        }
    }
}
