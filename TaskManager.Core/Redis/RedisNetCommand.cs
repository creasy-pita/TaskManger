using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Core.Redis
{
    /// <summary>
    /// redis网络命令发送
    /// </summary>
    public class RedisNetCommand
    {
        private string redisServerIp;

        public RedisNetCommand(string rediserverip)
        {
            redisServerIp = rediserverip;
        }
        /// <summary>
        /// 消息发送 屏蔽错误
        /// </summary>
        /// <param name="mqpath"></param>
        public void SendMessage(string msg)
        {
            var manager = new BSF.Redis.RedisManager();
            using (var c = manager.GetPoolClient(redisServerIp, RedisConfig.Redis_MaxConnectPoolSize, RedisConfig.Redis_MaxConnectPoolSize))
            {
                var i = c.GetClient().PublishMessage(RedisConfig.Redis_Channel, msg);
            }
        }


    }
}
