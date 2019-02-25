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
        public static string TaskConnectString = ConfigHelper.Configuration.GetValue<string>("TaskConnectString");
    }
}