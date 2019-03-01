using System;

namespace TaskManager.Node
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandQueueProcessor.ServiceRunning();
            //Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
