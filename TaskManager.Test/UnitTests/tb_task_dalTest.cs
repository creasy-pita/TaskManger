﻿using BSF.Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Web.Models;
using Xunit;

namespace TaskManager.Test.UnitTests
{
    public class tb_task_dalTest
    {
        [Fact]
        public void Nothing_JustGetJsonData()
        {
            string TaskConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
            using (DbConn PubConn = DbConn.CreateConn(TaskConnectString))
            {
                tb_task_dal dal = new tb_task_dal();
                PubConn.Open();
                tb_task_model task = dal.GetOneTask(PubConn, 2);
                tb_task_config_model task_Config_Model = new tb_task_config_model();
                FullTaskInfo fullTaskInfo = new FullTaskInfo();
                fullTaskInfo.config_models = new tb_task_config_model[] { task_Config_Model };
                fullTaskInfo.model = task;
                string aa = JsonConvert.SerializeObject(fullTaskInfo);
                Console.WriteLine(aa);
            }
            Console.WriteLine("ddddddd");
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
