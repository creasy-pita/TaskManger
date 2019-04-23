using BSF.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;

namespace TaskManager.Node.SystemMonitor
{
    /// <summary>
    /// 节点心跳监控者
    /// 用于心跳通知数据库当前节点状态
    /// </summary>
    public class NodeHeartBeatMonitor:BaseMonitor
    {
        public override int Interval
        {
            get
            {
                return 5000;
            }
        }
        protected override void Run()
        {
            SqlHelper.ExcuteSql(GlobalConfig.TaskDataBaseConnectString, (c) =>
            {
                var sqldatetime = c.GetServerDate();
                tb_node_dal nodedal = new tb_node_dal();
                tb_node_model node = nodedal.GetOneNode(c, GlobalConfig.NodeID);
                if (node != null)
                {
                    node.nodecreatetime = sqldatetime;
                    node.nodelastupdatetime = sqldatetime;
                    nodedal.AddOrUpdate(c, node);
                }
            });
            
        }
    }
}
