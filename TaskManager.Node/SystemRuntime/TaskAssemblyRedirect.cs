using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Node.SystemRuntime
{
    /// <summary>
    /// 任务程序集重定向
    /// </summary>
    public class TaskAssemblyRedirect
    {
        /// <summary>
        /// 用于兼容BSF.Full.dll SDk的使用方式
        /// </summary>
        /// <param name="fileinstallmainclassdllpath"></param>
        /// <param name="taskmainclassnamespace"></param>
        public void TryRebulidDll(string fileinstallmainclassdllpath,string taskmainclassnamespace)
        {
            AssemblyDefinition assembiy = AssemblyDefinition.ReadAssembly(fileinstallmainclassdllpath);
            TypeDefinition type = assembiy.MainModule.GetType(taskmainclassnamespace);
            if (type.BaseType != null && type.BaseType.Scope.Name=="BSF.Full")
            {
                
                var newtype = typeof(BSF.BaseService.TaskManager.BaseDllTask);
                var typeref = assembiy.MainModule.Import(newtype);
                type.BaseType = typeref;
            }
            
            assembiy.Write(fileinstallmainclassdllpath);
        }
    }
}
