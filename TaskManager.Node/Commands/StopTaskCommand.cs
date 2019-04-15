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
    /// 关闭任务命令
    /// </summary>
    public class StopTaskCommand:BaseCommand
    {
        public override void Execute()
        {
            tb_node_model node = new tb_node_model();
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (conn) =>
            {
                node = new tb_node_dal().Get(conn, GlobalConfig.NodeID);
            });
            string namespacestr = typeof(ITaskProvider).Namespace;
            string providerTypeName = $"{namespacestr}.{node.nodeostype}TaskProvider";
            ITaskProvider tp = (ITaskProvider)System.Reflection.Assembly
                .GetAssembly(typeof(ITaskProvider)).CreateInstance(providerTypeName);
            tp.Stop(this.CommandInfo.taskid);
        }
    }
}
