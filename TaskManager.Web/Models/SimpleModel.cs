using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Domain.Model;
using TaskManager.Web.Binders;

namespace TaskManager.Web.Models
{
    //TBD [ModelBinder(BinderType =typeof(FullTaskInfoBinder))]
    public class SimpleModel
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}
