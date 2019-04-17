using BSF.Db;
using System;
using System.Linq;
using System.Text;
using TaskManager.Node;
using Xunit;
using TaskManager.Domain.Dal;
using System.Collections.Generic;
using TaskManager.Domain.Model;

namespace TaskManager.Test.UnitTests
{
    public class TaskPerformanceMonitorTest
    {
        [Fact]
        public void SystemCollectionsGenericListClass_Where_ReturnIEnumerableListThatCanForeach()
        {
            List<tb_task_model> alltasks = new List<tb_task_model>();
            alltasks.Add(new tb_task_model { id=1,nodeid=3, taskstate=0});
            alltasks.Add(new tb_task_model { id = 2, nodeid =3, taskstate=1 });
            IEnumerable<tb_task_model> tasks = alltasks.Where((t) => t.taskstate == 1);
            Assert.Single(tasks);
        }
    }
}
