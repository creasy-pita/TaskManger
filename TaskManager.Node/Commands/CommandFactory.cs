using System;
using System.Diagnostics;

namespace TaskManager.Node.Commands
{
    public class CommandFactory
    {
        public static void Execute(string filePath)
        {
            try
            {
                var psi = new ProcessStartInfo(filePath)
                {
                    RedirectStandardOutput = true
                };
                var ProcessStart = Process.Start(psi);
                if (ProcessStart == null)
                {
                    throw new Exception("无法执行进程");
                }
                else
                {
                    var Output = ProcessStart.StandardOutput;
                    Console.WriteLine(Output.ReadLine());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
