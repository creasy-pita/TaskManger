using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace StartProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //Console.WriteLine("111 Main!");
            //Console.WriteLine("111 Main!");
            //Console.WriteLine("111 Main!");
            //Console.WriteLine("Hello nie!");

            string FileName = args[0];
            string Arguments = args[1];
            string workingDirectory = args[2];
            StartAProcessWithInfo(FileName, Arguments, workingDirectory);
        }

        static void StartAProcessWithInfo(string fileName, string Arguments, string workingDirectory)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = Arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
            };
            var ProcessStart = Process.Start(psi);
        }


    }
}
