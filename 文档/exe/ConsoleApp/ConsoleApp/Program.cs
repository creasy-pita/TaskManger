using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Hello World!");
            Console.WriteLine("111 Main!");
            Console.WriteLine("111 Main!");
            Console.WriteLine("111 Main!");
            Console.WriteLine("Hello nie!");
            //StartAProcess();

            string processName = args[0];
            string commandline = args[1];
            string proId = GetProcessId(processName, commandline);

            //string FileName = args[0];
            //string Arguments = args[1];
            //string workingDirectory = args[2];
            //StartAProcessWithInfo(FileName,Arguments, workingDirectory);
            //string proId = GetProcessId(FileName, Arguments);
            Console.WriteLine(proId);
        }

        static string GetProcessId(string processName,string commandline)
        {
            Process[] proclist = Process.GetProcessesByName(processName);
            foreach (var p in proclist)
            {
                string cmdName = ProcessCommonLine.GetCommandLineOfProcess(p);
                if (cmdName.IndexOf(commandline) > -1)
                {
                    return p.Id.ToString();
                }
            }
            return string.Empty;
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

        static void StartAProcess()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = @"E:\work\myproject\netcore\BackgroundTasksSample-GenericHost\bin\Debug\netcoreapp2.1\publish\BackgroundTasksSample.dll 11 222bb",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = @"E:\work\myproject\netcore\BackgroundTasksSample-GenericHost\bin\Debug\netcoreapp2.1\publish",
            };
            var ProcessStart = Process.Start(psi);
        }


    }

    internal class ProcessCommonLine
    {
        public static string GetCommandLineOfProcess(Process proc)
        {
            // max size of a command line is USHORT/sizeof(WCHAR), so we are going
            // just allocate max USHORT for sanity's sake.
            var sb = new StringBuilder(0xFFFF);
            switch (IntPtr.Size)
            {
                case 4: GetProcCmdLine32((uint)proc.Id, sb, (uint)sb.Capacity); break;
                case 8: GetProcCmdLine64((uint)proc.Id, sb, (uint)sb.Capacity); break;
            }
            return sb.ToString();
        }
        /// <summary>
        /// ProcCmdLine32.dll 非window 自带 是自定义，需要打包到发布包
        /// </summary>
        /// <param name="nProcId"></param>
        /// <param name="sb"></param>
        /// <param name="dwSizeBuf"></param>
        /// <returns></returns>
        [DllImport("ProcCmdLine32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetProcCmdLine")]
        public extern static bool GetProcCmdLine32(uint nProcId, StringBuilder sb, uint dwSizeBuf);

        [DllImport("ProcCmdLine64.dll", CharSet = CharSet.Unicode, EntryPoint = "GetProcCmdLine")]
        public extern static bool GetProcCmdLine64(uint nProcId, StringBuilder sb, uint dwSizeBuf);
    }
}
