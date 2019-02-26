using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Model;
using TaskManager.Domain.Dal;
using BSF.Db;
using TaskManager.Web.Models;
using BSF.Extensions;
using TaskManager.Core;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Web.Controllers
{
    [Authorize]
    public class UserController : BaseWebController
    {
        //
        // GET: /Developers/

        public ActionResult Index()
        {
            return this.Visit(EnumUserRole.Admin,() =>
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_user_model> Model = new tb_user_dal().GetAllUsers(PubConn);
                    return View(Model);
                }
            });
        }

        public ActionResult Add(int? userid)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                if (userid == null)
                    return View();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_user_dal dal = new tb_user_dal();
                    
                    var model = dal.Get(PubConn,userid.Value);
                    return View(model);
                }
            });
        }

        [HttpPost]
        public ActionResult Add(tb_user_model model)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_user_dal dal = new tb_user_dal();
                    model.usercreatetime = DateTime.Now;
                    model.usertel = model.usertel.NullToEmpty();
                    model.useremail = model.useremail.NullToEmpty();
                    if (model.id == 0)
                        dal.Add(PubConn, model);
                    else
                        dal.Edit(PubConn, model);
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
                    tb_user_dal dal = new tb_user_dal();
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
