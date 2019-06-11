using BSF.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class Config
    {
        public static string TaskConnectString = (ConfigHelper.Configuration==null)?"":ConfigHelper.Configuration.GetValue<string>("TaskConnectString");
        //public static string TaskConnectString = "Server=localhost;Database=dyd_bs_task;Uid=root;Pwd=123456;CharSet=utf8;";
    }
}