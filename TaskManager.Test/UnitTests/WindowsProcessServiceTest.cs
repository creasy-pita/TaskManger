using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Node.SystemRuntime.ProcessService;
using Xunit;

namespace TaskManager.Test.UnitTests
{
    public class WindowsProcessServiceTest
    {
        [Fact]
        public void test()
        {
            WindowsProcessService windowsProcessService = new WindowsProcessService();
            string batchScript = "cmd /c \"\"E:\\work\\project\\ZJZS.TaskManager\\文档\\开发\\BatchScript\\GetWinServiceProcessId-Via-CMD-ByServiceName.bat\"\" GtCxService_LSSJ1"; 
            string pId = windowsProcessService.GetProcessIdByBatchScript(batchScript);
            Assert.Equal("7696", pId);
        }
    }
}
