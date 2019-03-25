using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaskManager.Core;

namespace TaskManager.Node.SystemRuntime
{
    public class InitLinuxScript
    {
        static EnumSystemType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 安装服务脚本生成
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="access"></param>
        public static void InstallScript(string serviceName, string access)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{serviceName}";
            string filePath = $"{path}\\install.bat";
            if (!File.Exists(filePath))
            {
                



                string content = string.Format(GetContent("install"),
                    serviceName,
                    path,
                    access);
                Write(filePath, content);
            }
        }
        /// <summary>
        /// 卸载服务脚本
        /// </summary>
        /// <param name="serviceName"></param>
        public static void UninstallScript(string serviceName)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{serviceName}";
            string filePath = $"{path}\\uninstall.bat";
            if (!File.Exists(filePath))
            {
                string content = string.Format(GetContent("uninstall"), serviceName);
                Write(filePath, content);
            }
        }
        /// <summary>
        /// 开启服务脚本
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StartScript(string serviceName)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{serviceName}";
            string filePath = $"{path}\\start.bat";
            if (!File.Exists(filePath))
            {
                string content = string.Format(GetContent("start"), serviceName);
                Write(filePath, content);
            }
        }
        /// <summary>
        /// 停止服务脚本
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StopScript(string serviceName)
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}{serviceName}";
            string filePath = $"{path}\\stop.bat";
            if (!File.Exists(filePath))
            {
                string content = string.Format(GetContent("stop"), serviceName);
                Write(filePath, content);
            }
        }
        /// <summary>
        /// 将脚本写入文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">脚本</param>
        public static void Write(string path, string content)
        {
            try
            {
                StreamWriter file = new StreamWriter(path, false);
                content = content.Replace("\n", Environment.NewLine);
                file.Write(content);
                file.Close();
                file.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取XML中配置脚本
        /// </summary>
        /// <param name="OperationType"></param>
        /// <returns></returns>
        public static string GetContent(string OperationType)
        {
            string content = $@"
#!/bin/bash
dotnet BackgroundTasksSample.dll
";

            var ShellInfo = ShellConfig.Script.ShellInfos.Where(p => p.SystemType == Type.ToString());
            var Operation = ShellInfo.FirstOrDefault().Operations.Where(p => p.Type == OperationType);
            string Content = Operation.FirstOrDefault().Value;
            return Content;
        }
    }
}
