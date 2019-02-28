using TaskManager.Core;
using Xunit;

namespace TaskManager.Test
{
    public class CompressHelperTest
    {
        //[Fact]
        //public void CompressZip_FileCompressToZipAndUnCompressToFile_OutComeSameBytes()
        //{
        //    string file = @"E:\work\project\GisqSystem.TaskManager\TaskManager.Test\bin\Debug\netcoreapp2.1\netcoreapp2.0.zip";
        //    string dir = @"E:\work\project\GisqSystem.TaskManager\TaskManager.Test\bin\Debug\netcoreapp2.1\zip";
        //    CompressHelper.UnCompress(file, dir);
        //}
        [Fact]
        public void UnCompress_UnCompressConsoleAppZipToDll_CanExecuteRightly()
        {
            string file = @"E:\work\project\GisqSystem.TaskManager\TaskManager.Test\bin\Debug\netcoreapp2.1\netcoreapp2.0.zip";
            string dir = @"E:\work\project\GisqSystem.TaskManager\TaskManager.Test\bin\Debug\netcoreapp2.1\zip";
            CompressHelper.UnCompress(file, dir);
        }
    }
}
