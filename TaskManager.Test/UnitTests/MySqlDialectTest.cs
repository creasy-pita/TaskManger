using BSF.Db;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using System.Data;

namespace TaskManager.Test.UnitTests
{
    
    public class MySqlDialectTest
    {
        private string connectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";

        [Fact]
        public void SqlConnect_InputRightAccount_ReturnOK()
        {
            using (DbConn PubConn = DbConn.CreateConn(connectString))
            {
                PubConn.Open();
            }
        }
        [Fact]
        public void SqlGetConfigModel_InputCertainId_ReturnRightModel()
        {
            SqlHelper.ExcuteSql(this.connectString, (c) =>
            {
                tb_config_dal dal = new tb_config_dal();
                tb_config_model m = dal.Get(c, 1);
                //Assert.Equal("RedisServer", m.configkey);
                Assert.Equal("RedisServer  config url1", m.remark);
            });
        }

        #region  NodeMode

        [Fact]
        public void SqlAddOrUpdateNodeModel_InputTwoModel_ReturnRightOK()
        {
            SqlHelper.ExcuteSql(this.connectString, (c) =>
            {
                tb_node_dal dal = new tb_node_dal();
                tb_node_model m = new tb_node_model {
                    id = 6,
                    ifcheckstate = false,
                    nodecreatetime = DateTime.Now,
                    nodeip = "DESKTOP-N41U86J",
                    nodelastupdatetime = DateTime.Now,
                    nodename= "node001",
                    nodeostype= "Windows"
                };
                int r = dal.AddOrUpdate(c, m);
                Assert.Equal(1, r);
                r = 0;
                m = new tb_node_model
                {
                    id = 7,
                    ifcheckstate = false,
                    nodecreatetime = DateTime.Now,
                    nodeip = "linux.gisquest.com",
                    nodelastupdatetime = DateTime.Now,
                    nodename = "测试节点",
                    nodeostype = "Linux"
                };
                r = dal.AddOrUpdate(c, m);
                Assert.Equal(1, r);
            });
        }


        [Fact]
        public void SqlAddOrUpdateNodeModel_InputTwoModel_ReturnRightOK1()
        {
            SqlHelper.ExcuteSql(this.connectString, (c) =>
            {
                tb_node_dal dal = new tb_node_dal();
                tb_node_model m = new tb_node_model
                {
                    ifcheckstate = false,
                    nodecreatetime = DateTime.Now,
                    nodeip = "DESKTOP-N41U86J",
                    nodelastupdatetime = DateTime.Now,
                    nodename = "测试",
                    nodeostype = "Windows"
                };
                int r = dal.AddOrUpdate(c, m);
                Assert.Equal(1, r);

            });
        }

        /// <summary>
        /// SqlAddOrUpdateNodeModel_InputTwoModel_ReturnRightOK 先执行
        /// </summary>
        [Fact]
        public void SqlGetNodeModelList_InputTwoModel_ReturnRightOK()
        {
            SqlHelper.ExcuteSql(this.connectString, (c) =>
            {
                tb_node_dal dal = new tb_node_dal();
                int count = 0;
                List<tb_node_model> list = dal.GetList(c, null, null, null, 1, 2,out count);
                Assert.Single(list);
            });
        }
        
        #endregion
    }
}
