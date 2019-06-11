using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Model
{
    public class tb_webtasklist_model : tb_webtask_model
    {
        public string categoryname { get; set; }
        public string nodename { get; set; }
        public string username { get; set; }
    }
}
