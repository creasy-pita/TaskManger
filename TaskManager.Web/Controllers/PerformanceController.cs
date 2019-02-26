using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Web.Models;
using AspNetCorePage;
using BSF.Db;
using TaskManager.Core;

namespace TaskManager.Web.Controllers
{
    public class PerformanceController : BaseWebController
    {
        //
        // GET: /Performance/

        public ActionResult Index(string nodeid, string taskid, string orderby)
        {
            return this.Visit(EnumUserRole.None, () =>
            {
                ViewBag.nodeid = nodeid;
                ViewBag.taskid = taskid;
                ViewBag.orderby = orderby;
                tb_performance_dal dal = new tb_performance_dal();
                
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    ViewBag.taskmodels = dal.GetAllWithTask(PubConn,nodeid,taskid,orderby,DateTime.Now.AddMinutes(-10));
                   
                }
                return View();
            });
        }
        public ActionResult NodeIndex()
        {
            return this.Visit(EnumUserRole.None, () =>
            {
                tb_performance_dal dal = new tb_performance_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    ViewBag.nodemodels = dal.GetAllWithNode(PubConn, "p.nodeid desc", DateTime.Now.AddMinutes(-10));
                }
                return View();
            });
        }
    }
}
