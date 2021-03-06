﻿using BSF.Db;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node;
using TaskManager.Node.Commands;
using TaskManager.Node.SystemRuntime.ProcessService;
using TaskManager.Node.SystemRuntime.Services;
using TaskManager.Node.Tools;
using Xunit;

namespace TaskManager.Test.UnitTests
{
    public class TomcatServiceTestTest
    {

        [Fact]
        public void StartWebTaskCommandExecute_InputTaskId_ReturnNothingButTaskIsSuccessStarted()
        {
            GlobalConfig.TaskDataBaseConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
            StartWebTaskCommand command = new StartWebTaskCommand();
            command.CommandInfo = new tb_command_model { taskid = 1};
            command.Execute();
        }

        [Fact]
        public void SimpleTest()
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.WorkingDirectory = "F:\\softtempdata";//工作目录
            CmdProcess.StartInfo.FileName = @"java";      // 命令
            //CmdProcess.StartInfo.Arguments = "-jar F:\\softtempdata\\demo-0.0.1-SNAPSHOT.jar";      // 参数
            CmdProcess.StartInfo.Arguments = "-jar demo-0.0.1-SNAPSHOT.jar";      // 参数
            CmdProcess.EnableRaisingEvents = true;                      // 启用Exited事件
            CmdProcess.Start();
        }


        [Fact]
        public void Start_InputTomcatEntity_ReturnStartOk()
        {
            //TomcatEntity p = new TomcatEntity { Desc = "2222"
            //        , Path = "E:\\webServer\\tomcat\\apache-tomcat-8.0.39\\bin"
            //        , Port = "8080", TableName = "111"
            //        , HealthCheckUrl = "http://127.0.0.1:8080/FzTest/FzTest.jsp"
            //        , StartFileName = "E:\\webServer\\tomcat\\apache-tomcat-8.0.39\\bin\\startup.bat"
            //        , StartArguments="run"
            //    };
            //TomcatEntity p = new TomcatEntity
            //{
            //    Desc = "2222"
            //        ,
            //    Path = "F:\\softtempdata"
            //        ,
            //    Port = "9001",
            //    TableName = "111"
            //        ,
            //    HealthCheckUrl = "http://localhost:9001/test?name=1"
            //        ,
            //    StartFileName = "java"
            //        ,
            //    StartArguments = "-jar demo-0.0.1-SNAPSHOT.jar"
            //};
            TomcatEntity p = new TomcatEntity
            {
                Desc = "2222"
                    ,
                Path = "E:\\work\\myproject\\netcore\\MyWebAPI\\bin\\Debug\\netcoreapp2.0"
                    ,
                Port = "5000",
                TableName = "111"
                    ,
                HealthCheckUrl = "http://localhost:5000/api/values"
                    ,
                StartFileName = "dotnet"
                    ,
                StartArguments = "MyWebAPI.dll"
            };
            WebTaskProvider service = new WebTaskProvider();
            bool isOK = false;
            string isOKStr = "false";
            service.StartCompeleteEvent += (_ => { Console.WriteLine("I'm OK"); isOK = true; isOKStr = "true"; });
            service.Start(p);
            Thread.Sleep(20000);
            Console.WriteLine(isOK.ToString());
            Assert.Equal("true", isOKStr);
        }

        [Fact]
        public void Stop_InputTomcatEntity_ReturnStartOk()
        {
            //TomcatEntity t = new TomcatEntity
            //{
            //    Desc = "2222",
            //    Path = "E:\\webServer\\tomcat\\apache-tomcat-8.0.39\\bin"
            //        ,
            //    Port = "8080",
            //    TableName = "111"
            //        ,
            //    HealthCheckUrl = "http://127.0.0.1:8080/FzTest/FzTest.jsp"
            //};
            //TomcatEntity t = new TomcatEntity
            //{
            //    Desc = "2222"
            //        ,
            //    Path = "F:\\softtempdata"
            //        ,
            //    Port = "9001",
            //    TableName = "111"
            //        ,
            //    HealthCheckUrl = "http://localhost:9001/test?name=1"
            //        ,
            //    StartFileName = "java"
            //        ,
            //    StartArguments = "-jar demo-0.0.1-SNAPSHOT.jar"
            //};
            TomcatEntity t = new TomcatEntity
            {
                Desc = "2222"
                    ,
                Path = "E:\\work\\myproject\\netcore\\MyWebAPI\\bin\\Debug\\netcoreapp2.0"
                    ,
                Port = "5000",
                TableName = "111"
                    ,
                HealthCheckUrl = "http://localhost:5000/api/values"
                    ,
                StartFileName = "dotnet"
                    ,
                StartArguments = "MyWebAPI.dll"
            };
            WebTaskProvider service = new WebTaskProvider();
            service.Stop(t);
        }

        [Fact]
        public void Get_InputTomcatEntity_ReturnStartOk()
        {
            TomcatEntity t = new TomcatEntity
            {
                Desc = "2222",
                Path = "E:\\webServer\\tomcat\\apache-tomcat-8.0.39\\bin"
                    ,
                Port = "8080",
                TableName = "111"
                    ,
                HealthCheckUrl = "http://127.0.0.1:8080/FzTest/FzTest.jsp"
            };
            int i = 0;
            while (i++<10)
            {
                try
                {
                    (double cpu, long memory) = PerformanceHelper.GetPerformenceInfoByPort(t.Port);
                    Console.WriteLine($"cpu {cpu}; memory {memory}");
                    System.Threading.Thread.Sleep(2000);
                }
                catch (Exception exp)
                {
                }
            }


        }

    }
}
