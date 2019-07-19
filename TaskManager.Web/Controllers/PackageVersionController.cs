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
using Microsoft.AspNetCore.Http;

namespace TaskManager.Web.Controllers
{
    [Authorize]
    public class PackageVersionController : BaseWebController
    {
        //
        // GET: /Node/

        public ActionResult Index(int id, string CStime, string CEtime, int pagesize = 10, int pageindex = 1)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                ViewBag.sqldatetimenow = DateTime.Now;
                ViewBag.packageid = id;
                tb_packageversion_dal dal = new tb_packageversion_dal();
                PagedList<tb_packageversion_model> pageList = null;
                int count = 0;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_packageversion_model> List = dal.GetListByPackageId(PubConn, id, CStime, CEtime, pagesize, pageindex, out count);
                    pageList = new PagedList<tb_packageversion_model>(List, pageindex, pagesize, count);
                    ViewBag.sqldatetimenow = PubConn.GetServerDate();
                }
                return View(pageList);
            });
        }

        public ActionResult Add(int packageId)
        {
            ViewBag.packageid = packageId;
            return this.Visit(EnumUserRole.Admin, () =>
            {
                return View();
            });
        }

        [HttpPost]
        public ActionResult Add(tb_packageversion_model model, IFormFile TaskDll)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_packageversion_dal Dal = new tb_packageversion_dal();
                model.createtime = DateTime.Now;
                string filename = TaskDll.FileName;
                byte[] dllbyte;
                using (var dll = TaskDll.OpenReadStream())
                {
                    dllbyte = new byte[dll.Length];
                    dll.Read(dllbyte, 0, Convert.ToInt32(dll.Length));
                }
                model.zipfilename = filename;
                model.zipfile = dllbyte;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    Dal.AddOrUpdate(PubConn, model);
                }
                return RedirectToAction("index", routeValues: new { id=model.packageid});
            });
        }

        public ActionResult Update(int id)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_packageversion_dal dal = new tb_packageversion_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_packageversion_model model = dal.Get(PubConn, id);
                    return View(model);
                }
            });
        }

        [HttpPost]
        public ActionResult Update(tb_packageversion_model model, IFormFile TaskDll)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_packageversion_dal Dal = new tb_packageversion_dal();
                bool hasFileChanged = TaskDll != null;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_packageversion_model oldVersion = Dal.Get(PubConn, model.id);
                    model.zipfile = oldVersion.zipfile;
                    model.zipfilename = oldVersion.zipfilename;
                    model.packageid = oldVersion.packageid;
                    if(hasFileChanged)
                    {
                        model.zipfilename = TaskDll.FileName;
                        byte[] dllbyte;
                        using (var dll = TaskDll.OpenReadStream())
                        {
                            dllbyte = new byte[dll.Length];
                            dll.Read(dllbyte, 0, Convert.ToInt32(dll.Length));
                        }
                        model.zipfile = dllbyte;
                    }
                    Dal.Edit(PubConn, model);
                }
                return RedirectToAction("index", routeValues: new { id = model.packageid });
            });
        }

        public JsonResult Delete(int id)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                try
                {
                    tb_packageversion_dal dal = new tb_packageversion_dal();
                    using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                    {
                        PubConn.Open();
                        bool state = dal.Delete(PubConn, id);
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
