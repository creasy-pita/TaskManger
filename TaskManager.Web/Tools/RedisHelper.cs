using BSF.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.Core.Redis;
using TaskManager.Domain.Dal;
using TaskManager.Web.Models;

namespace TaskManager.Web.Tools
{
    public class RedisHelper
    {
        public static void SendMessage(Core.Redis.RedisCommondInfo commond)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RedisConfig.RedisServer))
                    RefreashRedisServerIP();
                if (string.IsNullOrWhiteSpace(RedisConfig.RedisServer))
                    throw new Exception("请在系统中“配置管理”中配置redis服务器的地址,配置key为:" + RedisConfig.RedisServerKey);

                new Core.Redis.RedisNetCommand(RedisConfig.RedisServer).SendMessage(new BSF.Serialization.JsonProvider().Serializer(commond));
            }
            catch (Exception exp)
            {

                LogHelper.AddError(new Domain.Model.tb_error_model()
                {
                    errorcreatetime = DateTime.Now,
                    errortype = (int)BSF.BaseService.TaskManager.SystemRuntime.EnumTaskLogType.SystemError,
                    msg = exp.DetailMessage(),
                    nodeid = commond.NodeId,
                    taskid = 0
                });
            }
        }

        public static void RefreashRedisServerIP()
        {
            try
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
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

                LogHelper.AddError(new Domain.Model.tb_error_model()
                {
                    errorcreatetime = DateTime.Now,
                    errortype = (int)BSF.BaseService.TaskManager.SystemRuntime.EnumTaskLogType.SystemError,
                    msg = string.Format("从配置中获取{0}出错,", RedisConfig.RedisServerKey)+exp.DetailMessage(),
                    nodeid = 0,
                    taskid = 0
                });
            }
        }
    }
}