using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TaskManager.Domain.Model;
using TaskManager.Node.Tools;

namespace TaskManager.Node.Commands
{
    public class CommandFactory
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandInfo"></param>
        public static void Execute(tb_command_model commandInfo)
        {
            string namespacestr = typeof(BaseCommand).Namespace;
            var obj = Assembly.GetAssembly(typeof(BaseCommand)).CreateInstance(namespacestr + "." + commandInfo.commandname.ToString() + "Command", true);
            if (obj != null && obj is BaseCommand)
            {
                var command = (obj as BaseCommand);
                command.CommandInfo = commandInfo;
                command.Execute();
            }
        }

        public static void Execute(string filePath)
        {
            try
            {
                var psi = new ProcessStartInfo(filePath)
                {
                    RedirectStandardOutput = true,
                    Verb= "runas"
                };
                var ProcessStart = Process.Start(psi);
                if (ProcessStart == null)
                {
                    throw new Exception("无法执行进程");
                }
                else
                {
                    var Output = ProcessStart.StandardOutput;
                    Console.WriteLine(Output.ReadLine());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LinuxExecute(string rootPath, string taskName, string serviceName)
        {
            LogHelper.AddTaskLog($"rootPath + taskName:{rootPath}/{taskName}",17);
            //new CommandFactory().ShowCurrentDirectory();
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $@"{serviceName}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                //RedirectStandardError = false,
                CreateNoWindow = true,
                WorkingDirectory = $"{rootPath}/{taskName}/",
            };
            var ProcessStart = Process.Start(psi);
            if (ProcessStart == null)
            {
                throw new Exception("无法执行进程");
            }
            else
            {
                var Output = ProcessStart.StandardOutput;
                Console.WriteLine(Output.ReadLine());
            }
            //System.Threading.Thread.Sleep(4000);
        }

        public  void ShowCurrentDirectory()
        {
            LogHelper.AddTaskLog($"System.AppContext.BaseDirectory:{System.AppContext.BaseDirectory}",17);
            LogHelper.AddTaskLog($"this.GetType().Assembly.Location:{GetType().Assembly.Location}",17);
            LogHelper.AddTaskLog($"Directory.GetCurrentDirectory():{Directory.GetCurrentDirectory()}",17);
            LogHelper.AddTaskLog($"AppDomain.CurrentDomain.BaseDirectory:{AppDomain.CurrentDomain.BaseDirectory}",17);
        }
        public static void LinuxExecute()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                //Arguments = @"path\release\PublishOutput\proces.dll",
                Arguments = @"/root/Software/netcore/ProcessStart/ConsoleApp/ConsoleApp.dll",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                CreateNoWindow = true,
                WorkingDirectory = @"/root/Software/netcore/ProcessStart/ConsoleApp/",
            };
            var ProcessStart = Process.Start(psi);
            if (ProcessStart == null)
            {
                throw new Exception("无法执行进程");
            }
            else
            {
                var Output = ProcessStart.StandardOutput;
                Console.WriteLine(Output.ReadLine());
            }
            System.Threading.Thread.Sleep(4000);
        }
    }
}
