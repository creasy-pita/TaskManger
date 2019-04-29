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
    public class tb_task_dalTest
    {

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
