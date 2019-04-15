using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TaskManager.Node.SystemRuntime
{
    class ProcessHelper
    {
        public static Process GetProcessById(int processId)
        {
            Process.GetProcessById(processId);
            return null;
        }

        public static string GetProcess(string serviceName)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "bash",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            pro.Start();
            //pro.StandardInput.WriteLine($"ps -A -f |grep 'dotnet {serviceName}'");
            pro.StandardInput.WriteLine($"ps aux | grep {serviceName} | grep -v grep | awk '{{print $2}}'");
            pro.StandardInput.WriteLine("exit");
            var line = "";
            var processId = "";
            while ((line = pro.StandardOutput.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line))
                {
                    processId = line;
                    //process = line.Substring(line.LastIndexOf(" ", StringComparison.Ordinal),
                    //    line.Length - line.LastIndexOf(" ", StringComparison.Ordinal));
                    break;
                }
            return processId;
        }
        /// <summary>
        /// 根据启动入口dll 名 获取进程id
        /// 注意：windows中 使用dotnet xx.dll 启动的程序集xx.dll并非进程名， 进程名是dotnet
        /// TBD 只处理进程名为dotnet 进程，其他类型暂时不加入
        /// </summary>
        /// <param name="serviceName">启动的入口程序集名称</param>
        /// <returns></returns>
        public static string GetWindowsProcess(string serviceName)
        {
            Process[] proclist = Process.GetProcessesByName("dotnet");
            foreach(var p in proclist)
            {
                string cmdName = ProcessCommonLine.GetCommandLineOfProcess(p);
                if (cmdName.IndexOf($"\"dotnet\" {serviceName}") > -1)
                {
                    return p.Id.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// TBD  kill  没有完全删除 进程， 需要进一步处理
        /// </summary>
        /// <param name="pId"></param>
        public static void KillWindowProcess(string pId)
        {
            try
            {
                Process proc = Process.GetProcessById(int.Parse(pId));
                proc.Kill();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// TBD  kill  没有完全删除 进程， 需要进一步处理
        /// </summary>
        /// <param name="pId"></param>
        public static void KillProcess(string pId)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "bash",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            pro.Start();
            //pro.StandardInput.WriteLine($"ps -A -f |grep 'dotnet {serviceName}'");
            pro.StandardInput.WriteLine($"kill {pId} ");
            pro.StandardInput.WriteLine("exit");
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
