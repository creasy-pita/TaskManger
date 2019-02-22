using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.Config
{
    public class ConfigHelper
    {
        public static string Get(string key, string defaultvalue = "")
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    return System.Configuration.ConfigurationManager.AppSettings[key];
                }
                else
                {
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
