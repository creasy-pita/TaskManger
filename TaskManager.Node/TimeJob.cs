using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Node
{
    public class TimeJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            CheckState.CheckRunning();
            return null;
        }
    }
}
