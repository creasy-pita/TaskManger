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
using TaskManager.Node.SystemRuntime.Services;

namespace TaskManager.Node.Commands
{
    /// <summary>
    /// 开启任务命令
    /// </summary>
    public class StartWebTaskCommand : BaseCommand
    {
        public override void Execute()
        {
            //TBD 默认 一个任务信息entity, 后续通过taskid 获取具体的任务信息entity
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
            TomcatService tomcatService = new TomcatService();
            tomcatService.Start(t);

            //string namespacestr = typeof(ITaskProvider).Namespace;
            //string providerTypeName = $"{namespacestr}.{node.nodeostype}TaskProvider";
            //ITaskProvider tp = (ITaskProvider)System.Reflection.Assembly
            //    .GetAssembly(typeof(ITaskProvider)).CreateInstance(providerTypeName);
            //tp.Start(this.CommandInfo.taskid);
        }
    }
}
