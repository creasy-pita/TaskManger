using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Node.Exceptions
{
    class TaskAlreadyRunningException:Exception
    {
        public TaskAlreadyRunningException(string message):base(message)
        {
        }
    }
}
