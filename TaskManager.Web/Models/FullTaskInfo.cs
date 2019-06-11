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
    [ModelBinder(BinderType =typeof(FullTaskInfoBinder))]
    public class FullTaskInfo
    {
        //public ActionResult Add(IFormFile TaskDll, tb_task_model model, string tempdatajson)

        public IFormFile TaskDll { get; set; }

        public tb_task_model model { get; set; }

        public tb_task_config_model[] config_models { get; set; }
        public string tempdatajson { get; set; }
    }
}
