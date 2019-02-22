using BSF.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TaskManager.Core.Redis;
using TaskManager.Domain.Dal;

namespace TaskManager.Node.Tools
{
    public class RedisHelper
    {
        public static void RedisListner(Action<string, string> action, Action<RedisErrorInfo> errorAction)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RedisConfig.RedisServer))
                    RefreashRedisServerIP();
                if (string.IsNullOrWhiteSpace(RedisConfig.RedisServer))
                    throw new Exception("请在系统中“配置管理”中配置redis服务器的地址,配置key为:" + RedisConfig.RedisServerKey);
                var cancelSource = new CancellationTokenSource();
                //LogHelper.AddNodeError($"{RedisConfig.RedisServer}", new Exception());

                new RedisNetCommandListener(RedisConfig.RedisServer).Register(action,errorAction,
                    cancelSource, RedisConfig.Redis_Channel);
            }
            catch (Exception exp)
            {
                LogHelper.AddNodeError("订阅redis出错", exp);
            }
            
        }
        public static void RefreashRedisServerIP()
        {
            try
            {
                using (DbConn PubConn = DbConn.CreateConn(GlobalConfig.TaskDataBaseConnectString))
                {
                    PubConn.Open();
                    var dal = new tb_config_dal();
                    var config = dal.Get(PubConn, RedisConfig.RedisServerKey);
                    if (config != null)
                        RedisConfig.RedisServer = config.configvalue;
                }
            }
            catch (Exception exp)
            {
                LogHelper.AddNodeError(string.Format("从配置中获取{0}出错,", RedisConfig.RedisServerKey),exp);
            }
        }
    }
}
