using BSF.Config;
using BSF.Redis;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Model;
using TaskManager.Node;
using TaskManager.Web.Controllers;
using TaskManager.Web.Models;
using Xunit;

namespace TaskManager.Test.UnitTests
{
    public class CommandControllerTest
    {
        [Fact]
        public void ConfigHelperConfigurationSet_SetByConfigurationBuilderBind_ReturnOK()
        {
            ConfigHelper.Configuration = GetIConfigurationRoot(AppDomain.CurrentDomain.BaseDirectory);
            //string conStr = ConfigHelper.Configuration.GetValue<string>("TaskConnectString");
            string conStr = Config.TaskConnectString;
            Assert.Equal("Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;", conStr);
            //Assert.Null(conStr);
        }

        [Fact]
        public void StartWebTaskCommandExecute_InputTaskId_ReturnNothingButTaskIsSuccessStarted()
        {
            ConfigHelper.Configuration = GetIConfigurationRoot(AppDomain.CurrentDomain.BaseDirectory);

            TaskManager.Core.Redis.RedisConfig.RedisServer = "192.168.5.217";
            GlobalConfig.TaskDataBaseConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
            CommandController commandController = new CommandController();
            commandController.Add(new tb_command_model
            {
                command =string.Empty,
                nodeid = 3,
                taskid = 1,
                tasktype = 1,
                commandstate = 0,
                commandname = "StartWebTask",
                commandcreatetime = DateTime.Now
            });

        }

        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }

    }
}
