using System;
using System.Threading;

namespace TaskManager.Node
{
    class Program
    {
        static int period = 20;
        static string identityName = "ZS.Service.Manager";
        static void Main(string[] args)
        {
            Thread t1 = new Thread(CommandQueueProcessor.ServiceRunning);
            t1.Start();
            Thread t2 = new Thread(Monitor);
            t2.Start();
            Console.ReadKey();
        }
        static void Monitor()
        {
            ScheduleJob.ExecuteInterval<TimeJob>(period, identityName);
        }
    }
}
