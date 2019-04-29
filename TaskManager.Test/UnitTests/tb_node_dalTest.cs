using BSF.Db;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using Xunit;

namespace TaskManager.Test.UnitTests
{
    public class tb_node_dalTest
    {
        [Fact]
        public void tt()
        {
            string TaskConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
            using (DbConn PubConn = DbConn.CreateConn(TaskConnectString))
            {
                tb_node_dal dal = new tb_node_dal();
                PubConn.Open();
                int count;
                List<tb_node_model> List = dal.GetList(PubConn, null, null, null, 2, 1, out count);
                Console.WriteLine("list.count" + List.Count);
            }
        }
    }
}
