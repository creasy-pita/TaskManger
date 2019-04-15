﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core
{
    /// <summary>
    /// 任务命令名称
    /// </summary>
    public enum EnumTaskCommandName
    {
        [Description("关闭任务")]
        StopTask=0,
        [Description("开启任务")]
        StartTask=1,
        [Description("重启任务")]
        ReStartTask=2,
        [Description("卸载任务")]
        UninstallTask=3,
    }
    /// <summary>
    /// 任务命令状态
    /// </summary>
    public enum EnumTaskCommandState
    {
        [Description("未执行")]
        None=0,
        [Description("执行错误")]
        Error=1,
        [Description("成功执行")]
        Success=2
    }
    /// <summary>
    /// 任务执行状态
    /// </summary>
    public enum EnumTaskState
    {
        [Description("未安装")]
        UnInstall=-1,
        [Description("停止")]
        Stop=0,
        [Description("运行中")]
        Running=1,      
    }
    /// <summary>
    /// 任务执行状态
    /// </summary>
    public enum EnumOSState
    {
        [Description("Windows")]
        Windows = 0,
        [Description("Linux")]
        Linux = 1,
    }

    /// <summary>
    /// 系统用户角色
    /// </summary>
    public enum EnumUserRole
    {
        [Description("管理员")]
        Admin=0,
        [Description("开发人员")]
        Developer=1,
        [Description("无控制")]
        None = -1
    }

    public enum EnumSystemType
    {
        [Description("windows系统")]
        windows = 0,
        [Description("Linux系统")]
        linux = 1,
        [Description("未知系统")]
        unknown = 2,
    }
}
