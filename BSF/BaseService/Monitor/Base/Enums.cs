using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSF.BaseService.Monitor
{

    public enum EnumTimeWatchLogType
    {
        [Description("无")]
        None = 0,
        [Description("普通")]
        Common = 1,
        [Description("API")]
        ApiUrl = 2,
        [Description("SQL")]
        SqlCmd = 3,
    }

    public enum EnumLogType
    {
        [Description("普通错误")]
        CommonError = 1,
        [Description("系统错误")]
        SystemError = 2,
        [Description("普通")]
        CommonLog = 3,
        [Description("系统")]
        SystemLog = 4,
    }

    public enum EnumCommonLogType
    {
        CommonLog = 3,
        SystemLog = 4,
    }

    public enum EnumErrorLogType
    {
        [Description("普通错误")]
        CommonError = 1,
        [Description("系统错误")]
        SystemError = 2,
    }
}
