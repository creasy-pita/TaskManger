using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TaskManager.Node.SystemRuntime
{
    [XmlType(TypeName = "Root")]
    public class Shell
    {
        [XmlElement("ShellInfo")]
        public List<ShellInfo> ShellInfos
        {
            get;
            set;
        }
    }
    [XmlType(TypeName = "ShellInfo")]
    public class ShellInfo
    {
        [XmlAttribute]
        public string SystemType
        {
            get;
            set;
        }
        [XmlElement("Operation")]
        public List<Operation> Operations
        {
            get;
            set;
        }
    }
    [XmlType(TypeName = "Operation")]
    public class Operation
    {
        [XmlAttribute]
        public string Type
        {
            get;
            set;
        }
        [XmlElement("Value")]
        public string Value
        {
            get;
            set;
        }
    }
}
