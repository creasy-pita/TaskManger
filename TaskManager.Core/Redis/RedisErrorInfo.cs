using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.Core.Redis
{
    public class RedisErrorInfo
    {
        public string ChannelName { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
