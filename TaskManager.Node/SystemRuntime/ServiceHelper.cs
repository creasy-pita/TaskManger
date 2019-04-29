using System.Linq;
using System.ServiceProcess;
using TaskManager.Core;

namespace TaskManager.Node.SystemRuntime
{
    public class ServiceHelper
    {
        public static EnumTaskState ServiceState(string ServiceName)
        {
            var ServiceControllers = ServiceController.GetServices();
            var Service = ServiceControllers.FirstOrDefault(s => s.ServiceName == ServiceName);
            if (Service == null)
            {
                return EnumTaskState.Stop;
            }
            else if (Service.Status == ServiceControllerStatus.Running)
            {
                return EnumTaskState.Running;
            }
            else
            {
                return EnumTaskState.Stop;
            }
        }
    }
}
