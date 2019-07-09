using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Node.SystemRuntime.ProcessService
{
    public interface IProcessService
    {
        string GetProcessByPort(string port);
        string GetProcessByName(string serviceName);
        string GetProcessIdByBatchScript(string batchScript);
        string GetProcessByCondition(string processName, string commandLineFilterStr);
        void ProcessKill(int pId);
    }
}
