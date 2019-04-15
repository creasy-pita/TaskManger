using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core;
using TaskManager.Node.SystemRuntime;
using Xunit;
namespace TaskManager.Test.UnitTests
{
    public class TaskProviderTest
    {
        [Fact]
        public void CreateTaskProviderImpInstanceByReflection_InputWindowsProviderType_ReturnWindowsProviderType()
        {
            string osType = "Windows";
            ITaskProvider tp = (ITaskProvider)System.Reflection.Assembly
                .GetAssembly(typeof(ITaskProvider))
                .CreateInstance($"TaskManager.Node.SystemRuntime.{osType}TaskProvider");
            Assert.IsNotType<WindowsTaskProvider>(tp);
        }


        [Fact]
        public void EnumToString_ReturnEnumObjectNameString()
        {
            Assert.Equal("Windows", EnumOSState.Linux.ToString());
        }

        [Fact]
        public void EnumParse_InputEnumObjectValueString_ReturnRightEnumObject()
        {
            string nodeOSType = "0";
            EnumOSState s = (EnumOSState)Enum.Parse(typeof(EnumOSState), nodeOSType);
            Assert.Equal("Windows", s.ToString());
        }

        [Fact]
        public void EnumParse_InputEnumObjectNameString_ReturnRightEnumObject()
        {
            string nodeOSType = "Linux";
            EnumOSState s = (EnumOSState)Enum.Parse(typeof(EnumOSState), nodeOSType);
            Assert.Equal("Linux", s.ToString());
        }
    }
}
