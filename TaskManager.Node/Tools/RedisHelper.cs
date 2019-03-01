using BSF.Db;
using System;
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
                {
                    RefreashRedisServerIP();
                }
                if (string.IsNullOrWhiteSpace(RedisConfig.RedisServer))
                {
                    throw new Exception("Redis为配置服务器地址");
                }
                var cancelSource = new CancellationTokenSource();
                new RedisNetCommandListener(RedisConfig.RedisServer).Register(action, errorAction,
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
                using (DbConn PubConn = DbConn.CreateConn(GlobalConfig.ConnectionString))
                {
                    PubConn.Open();
                    var dal = new tb_config_dal();
                    var config = dal.Get(PubConn, RedisConfig.RedisServerKey);
                    if (config != null)
                    {
                        RedisConfig.RedisServer = config.configvalue;
                    }
                }
            }
            catch (Exception exp)
            {
                LogHelper.AddNodeError(string.Format("从配置中获取{0}出错,", RedisConfig.RedisServerKey), exp);
            }
        }
    }
}
