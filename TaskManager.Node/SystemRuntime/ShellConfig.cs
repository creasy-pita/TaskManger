using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TaskManager.Node.SystemRuntime
{
    public class ShellConfig
    {
        public static Shell _script;
        public static Shell Script
        {
            get
            {
                if (_script == null)
                {
                    _script = XmlDeserialize();
                }
                return _script;
            }
        }
        public static Shell XmlDeserialize()
        {
            string Path = $"{AppDomain.CurrentDomain.BaseDirectory}Shell.xml";
            if (File.Exists(Path))
            {
                using (StreamReader reader = new StreamReader(Path))
                {
                    try
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Shell));
                        return (Shell)xmlSerializer.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                throw new Exception($"配置文件：{Path}不存在");
            }
        }
    }
}
