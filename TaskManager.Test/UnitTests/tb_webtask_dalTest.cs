using BSF.Db;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using Xunit;

namespace TaskManager.Test.UnitTests
{
    public class tb_webtask_dalTest
    {

        [Fact]
        public void AddTask_InputOneTask_ReturnOk()
        {
            string TaskConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
            using (DbConn PubConn = DbConn.CreateConn(TaskConnectString))
            {
                tb_webtask_dal dal = new tb_webtask_dal();
                PubConn.Open();

                tb_webtask_model task = new tb_webtask_model();
                task.categoryid = 1;
                //task.id =
                task.nodeid = 3;
                task.taskcreatetime = DateTime.Now;
                task.taskname = "mynetcorewebapi";
                task.taskupdatetime = DateTime.Now;
                task.taskerrorcount = 0;
                task.taskruncount = 0;
                task.taskcreateuserid = 1;
                task.taskstate = 0;
                task.taskremark = string.Empty;
                task.taskport = 5000;
                task.taskhealthcheckurl = "http://localhost:5000/api/values";
                task.taskpath = "E:\\work\\myproject\\netcore\\MyWebAPI\\bin\\Debug\\netcoreapp2.0";
                task.taskstartfilename = "dotnet";
                task.taskarguments = "MyWebAPI.dll";
                Assert.True(dal.Add(PubConn, task));
            }
        }


        [Fact]
        public void UpdateTask_InputStateAndStartTime_ReturnWorkOk()
        {
            string TaskConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
            using (DbConn PubConn = DbConn.CreateConn(TaskConnectString))
            {
                tb_task_dal dal = new tb_task_dal();
                PubConn.Open();
                tb_task_model task = dal.GetOneTask(PubConn, 2);
                task.tasklaststarttime = DateTime.Now;
                task.taskstate = (byte)EnumTaskState.Running;
                dal.UpdateTask(PubConn, task);
            }
        }

        [Fact]
        public void EnumConvertToByte()
        {
            Byte a = (byte)EnumTaskState.Running;
            Byte b = (byte)EnumTaskState.Stop;
            Assert.Equal("10", a.ToString() + b.ToString());
        }
    }
}
