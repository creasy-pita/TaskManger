

namespace TaskManager.Node.SystemRuntime.ProcessService
{
    public class ProcessServiceFactory
    {
        public static IProcessService CreateProcessService(string OSTypeName)
        {
            string namespacestr = typeof(IProcessService).Namespace;
            string typeName = $"{namespacestr}.{OSTypeName}ProcessService";
            IProcessService ps = (IProcessService)System.Reflection.Assembly
                .GetAssembly(typeof(IProcessService)).CreateInstance(typeName);
            return ps;
        }
    }
}
