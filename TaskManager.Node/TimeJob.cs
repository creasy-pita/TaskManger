using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Node
{
    public class TimeJob : IJob
    {
        private static object obj= new object();
        public Task Execute(IJobExecutionContext context)
        {
            lock (obj)
            {
                CheckState.CheckRunning();
                return null;
            }
        }


    }
}
