﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TaskManager.Core.Util;
using TaskManager.Node.Tools;

namespace TaskManager.Node.SystemRuntime.ProcessService
{
    public class WindowsProcessService : IProcessService
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
        /// <summary>
        /// 按进程名称 和 命令行参数查找 进程id
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <param name="commandLineFilterStr">进程的命令行参数</param>
        /// <returns></returns>
        public string GetProcessByCondition(string processName, string commandLineFilterStr)
        {
            Process[] proclist = Process.GetProcessesByName(processName);
            foreach (var p in proclist)
            {
                string cmdName = ProcessCommonLine.GetCommandLineOfProcess(p);
                if (cmdName.IndexOf(commandLineFilterStr) > -1)
                {
                    return p.Id.ToString();
                }
            }
            return string.Empty;
        }


        public string RunBatchScriptWithCmd(string batchScript)
        {

            return null;
        }

        public string GetProcessIdByBatchScript(string batchScript)
        {
            string processId = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(batchScript)) return null;
                var pro = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",// "powershell.exe",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                pro.Start();
                // pro.StandardInput.WriteLine("Get-WmiObject win32_service -filter \"name = 'GtCxService_LSSJ'\"| Select-Object -ExpandProperty ProcessId ");
                pro.StandardInput.WriteLine(batchScript);
                pro.StandardInput.Flush();
                pro.StandardInput.WriteLine("exit");
                processId = GetThirdLastLine(pro.StandardOutput);
                //if substring of the string endwiths batchScript means no result,so you should avoid batchScript of ProssesId-like
                //TBD 此处需要注意 batchScript 保存的内容， 不能出现纯数字，或者0（如果保存的时此类内容则直接忽略 返回 null）
                if (processId.EndsWith(batchScript) || processId == "0")
                {
                    processId = string.Empty;
                }
                if(!CastUtil.CanCastInt(processId) )
                {
                    processId = string.Empty;
                }
                return processId;
            }
            catch (Exception ex)
            {
                LogHelper.AddNodeLog($"GetProcessIdByBatchScript出错 {ex.Message + ex.StackTrace}");
                throw ex;
            }
        }

        public static string GetThirdLastLine(StreamReader streamReader)
        {
            var line = "";
            Stack lineStack = new Stack();
            while ((line = streamReader.ReadLine()) != null)
            {
                lineStack.Push(line);
            }
            if (lineStack.Pop() != null && lineStack.Pop() !=null)
            {
                if ((line = lineStack.Pop().ToString()) != null)
                {
                    return line;
                }
                else
                    return null;
            }
            else
                return null;
        }


        public string GetProcessByPort(string port)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            pro.Start();
            pro.StandardInput.WriteLine("netstat -ano | findstr " + port);
            pro.StandardInput.WriteLine("exit");
            var line = "";
            var process = "";
            while ((line = pro.StandardOutput.ReadLine()) != null)
                if (!string.IsNullOrEmpty(line) && line.IndexOf("TCP", StringComparison.Ordinal) > -1 && line.IndexOf($":{port} ", StringComparison.Ordinal)>-1 && line.IndexOf($"LISTENING", StringComparison.Ordinal) > -1)
                {
                    process = line.Substring(line.LastIndexOf(" ", StringComparison.Ordinal),
                        line.Length - line.LastIndexOf(" ", StringComparison.Ordinal));
                    break;
                }
            return process;
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
