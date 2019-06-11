using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Node.Exceptions
{
    /// <summary>
    /// 任务已经在运行中
    /// </summary>
    class TaskAlreadyNotRunningException:Exception
    {
        public TaskAlreadyNotRunningException(string message):base(message)
        {
        }
    }
}
