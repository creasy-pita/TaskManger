using BSF.BaseService.TaskManager.Dal;
using BSF.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Node.SystemRuntime;

namespace TaskManager.Node.Commands
{
    /// <summary>
    /// 开启任务命令
    /// </summary>
    public class TomcateStartTaskCommand : BaseCommand
    {
        public override void Execute()
        {
            tb_node_model node = new tb_node_model();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                node = new tb_node_dal().Get(conn, GlobalConfig.NodeID);
            });


            //tomcatservice 

            string Path = "E:\\webServer\\tomcat\\apache-tomcat-8.0.39\\bin";
            string Port = "8080";
            string TableName = "111";
            string HealthCheckUrl = "http://127.0.0.1:8080/FzTest/FzTest.jsp";



        }
    }
}
