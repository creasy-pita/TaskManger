using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Core.Redis
{
    public class RedisCommondInfo
    {
        public EnumCommondType CommondType {get;set;}
        public int NodeId { get; set; }
    }

    public enum EnumCommondType
    {
        ConfigUpdate=1,
        TaskCommand=2,
    }
}
