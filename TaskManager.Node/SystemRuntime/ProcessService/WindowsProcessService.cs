using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TaskManager.Node.SystemRuntime.ProcessService
{
    class WindowsProcessService : IProcessService
    {
        public string GetProcessByName(string serviceName)
        {
            Process[] proclist = Process.GetProcessesByName("dotnet");
            foreach (var p in proclist)
            {
                string cmdName = ProcessCommonLine.GetCommandLineOfProcess(p);
                if (cmdName.IndexOf($"\"dotnet\" {serviceName}") > -1)
                {
                    return p.Id.ToString();
                }
            }
            return string.Empty;
        }

        public void ProcessKill(int pId)
        {
            Process proc = Process.GetProcessById(pId);
            proc.Kill();
            //try
            //{
            //    Process proc = Process.GetProcessById(pId);
            //    proc.Kill();
            //}
            //catch (Exception ex)
            //{
            //}
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
