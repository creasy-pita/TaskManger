using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Node.SystemRuntime
{
    public interface ITaskProvider
    {
        bool Start(int taskid);
        bool Stop(int taskid);
        bool Uninstall(int taskid);
    }
}
