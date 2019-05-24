using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Node.SystemRuntime.ProcessService
{
    public interface IProcessService
    {
        string GetProcessByPort(string port);
        string GetProcessByName(string serviceName);
        void ProcessKill(int pId);
    }
}
