using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TaskManager.Node.SystemRuntime
{
    class ProcessHelper
    {
        public static string GetProcess(string serviceName)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "bash",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            pro.Start();
            //pro.StandardInput.WriteLine($"ps -A -f |grep 'dotnet {serviceName}'");
            pro.StandardInput.WriteLine($"ps aux | grep {serviceName} | grep -v grep | awk '{{print $2}}'");
            pro.StandardInput.WriteLine("exit");
            var line = "";
            var process = "";
            while ((line = pro.StandardOutput.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    process = line;
                    //process = line.Substring(line.LastIndexOf(" ", StringComparison.Ordinal),
                    //    line.Length - line.LastIndexOf(" ", StringComparison.Ordinal));
                    break;
                }
            return process;
        }
        /// <summary>
        /// TBD  kill  没有完全删除 进程， 需要进一步处理
        /// </summary>
        /// <param name="pId"></param>
        public static void KillProcess(string pId)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "bash",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            pro.Start();
            //pro.StandardInput.WriteLine($"ps -A -f |grep 'dotnet {serviceName}'");
            pro.StandardInput.WriteLine($"kill {pId} ");
            pro.StandardInput.WriteLine("exit");
        }
    }
}
