using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BSF.Config
{
    public static class ConfigHelper
    {
        public static IConfiguration Configuration;

        public static string Get(string key, string defaultvalue = "")
        {
            try
            {
                //if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(key))
                //{
                //    return System.Configuration.ConfigurationManager.AppSettings[key];
                //}
                if (!string.IsNullOrEmpty( Configuration.GetValue<string>(key)))
                {
                    return Configuration.GetValue<string>(key);
                }
                else
                {//TBD 一下可能会报错，需要调整到 Microsoft.Extensions.Configuration。IConfiguration方式
                    if (BaseService.BaseServiceContext.ConfigManagerProvider != null)
                    {
                        string value = null;
                        if (BaseService.BaseServiceContext.ConfigManagerProvider.TryGet<string>(key, out value) == true)
                        {
                            return value;
                        }
                        else
                        {
                            return defaultvalue;
                        }
                    }
                }
            }
            catch
            {
                return defaultvalue;
            }

            return defaultvalue;
        }
        
    }
}
