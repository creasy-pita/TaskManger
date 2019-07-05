using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TaskManager.Node.SystemRuntime.ProcessService
{
    class LinuxProcessService : IProcessService
    {
        public string GetProcessByName(string serviceName)
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
            pro.StandardInput.WriteLine($"ps aux | grep {serviceName} | grep -v grep | awk '{{print $2}}'");
            pro.StandardInput.WriteLine("exit");
            var line = "";
            var processId = "";
            while ((line = pro.StandardOutput.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    processId = line;
                    break;
                }
            return processId;
        }

        public string GetProcessByPort(string port)
        {
            return "";
        }

        public string GetProcessIdByBatchScript(string batchScript)
        {
            throw new NotImplementedException();
        }

        public void ProcessKill(int pId)
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
