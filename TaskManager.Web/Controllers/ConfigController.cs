using BSF.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core;
using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Web.Models;
using BSF.Extensions;
using TaskManager.Core.Redis;
using TaskManager.Web.Tools;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Web.Controllers
{
    [Authorize]
    public class ConfigController : BaseWebController
    {
        //
        // GET: /Developers/

        public ActionResult Index()
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_config_model> Model = new tb_config_dal().GetList(PubConn);
                    return View(Model);
                }
            });
        }

        public ActionResult Add(int? id)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                if (id == null)
                    return View();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_config_dal dal = new tb_config_dal();

                    var model = dal.Get(PubConn, id.Value);
                    return View(model);
                }
            });
        }

        [HttpPost]
        public ActionResult Add(tb_config_model model)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_config_dal dal = new tb_config_dal();
                    model.lastupdatetime = DateTime.Now;
                    model.configkey = model.configkey.NullToEmpty();
                    model.configvalue = model.configvalue.NullToEmpty();
                    model.remark = model.remark.NullToEmpty();
                    if (model.id == 0)
                        dal.Add(PubConn, model);
                    else
                        dal.Edit(PubConn, model);
                    RedisHelper.RefreashRedisServerIP();
                    RedisHelper.SendMessage(new RedisCommondInfo() { CommondType = EnumCommondType.ConfigUpdate });
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
                    tb_config_dal dal = new tb_config_dal();
                    using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                    {
                        PubConn.Open();
                        bool state = dal.Delete(PubConn, id);
                        RedisHelper.RefreashRedisServerIP();
                        RedisHelper.SendMessage(new RedisCommondInfo() { CommondType = EnumCommondType.ConfigUpdate });
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
