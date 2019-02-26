using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using BSF.Db;
using TaskManager.Web.Models;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using AspNetCorePage;
using TaskManager.Core.Net;
using TaskManager.Core;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Web.Controllers
{
    [Authorize]
    public class NodeController : BaseWebController
    {
        //
        // GET: /Node/

        public ActionResult Index(string keyword, string CStime, string CEtime, int pagesize = 10, int pageindex = 1)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                ViewBag.sqldatetimenow = DateTime.Now;
                tb_node_dal dal = new tb_node_dal();
                PagedList<tb_node_model> pageList = null;
                int count = 0;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_node_model> List = dal.GetList(PubConn, keyword, CStime, CEtime, pagesize, pageindex, out count);
                    pageList = new PagedList<tb_node_model>(List, pageindex, pagesize, count);
                    ViewBag.sqldatetimenow = PubConn.GetServerDate();
                }
                return View(pageList);
            });
        }

        public ActionResult Add()
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                return View();
            });
        }

        [HttpPost]
        public ActionResult Add(tb_node_model model)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_node_dal Dal = new tb_node_dal();
                model.nodecreatetime = DateTime.Now;
                model.nodelastupdatetime = DateTime.Now;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    Dal.AddOrUpdate(PubConn, model);
                }
                return RedirectToAction("index");
            });
        }

        public ActionResult Update(int id)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_node_dal dal = new tb_node_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_node_model model = dal.GetOneNode(PubConn, id);
                    return View(model);
                }
            });
        }

        [HttpPost]
        public ActionResult Update(tb_node_model model)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_node_dal Dal = new tb_node_dal();
                model.nodecreatetime = DateTime.Now;
                model.nodelastupdatetime = DateTime.Now;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    Dal.Update(PubConn, model);
                }
                return RedirectToAction("index");
            });
        }

        public JsonResult Delete(int id)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                try
                {
                    tb_node_dal dal = new tb_node_dal();
                    using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                    {
                        PubConn.Open();
                        bool state = dal.DeleteOneNode(PubConn, id);
                        return Json(new { code = 1, state = state });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { code = -1, msg = ex.Message });
                }
            });
        }

        
    }
}
