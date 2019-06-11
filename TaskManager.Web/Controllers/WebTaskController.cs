using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Model;
using TaskManager.Domain.Dal;
using BSF.Db;
using TaskManager.Web.Models;
using AspNetCorePage;
using System.Drawing;
using System.IO;
using TaskManager.Core;
using BSF.Extensions;
using TaskManager.Web.Tools;
using System.Data.SqlClient;
using System.Data;
using BSF.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace TaskManager.Web.Controllers
{
    //[Authorize]//TBD 需要取消[Authorize]/ 的注释
    public class WebTaskController : BaseWebController
    {
        //
        // GET: /WebTask/

        public ActionResult Index(string taskid, string keyword, string CStime, string CEtime, int categoryid = -1, int nodeid = -1, int userid = -1, int state = -999, int pagesize = 10, int pageindex = 1)
        {
            return this.Visit(EnumUserRole.None, () =>
            {
                #region 保存查询信息,优化操作体验
                var ps = Request.RequestParams();
                ps.Remove("userid");
                var sessionkey = "/webtask/index/";
                if (ps.Count == 0)
                {
                    if (HttpContext.Session.GetString(sessionkey) != null)
                    {
                        var ks = new BSF.Serialization.JsonProvider( BSF.Serialization.JsonAdapter.EnumJsonMode.Newtonsoft).Deserialize<List<KeyValuePair<string,object>>>(HttpContext.Session.GetString(sessionkey));
                        foreach (var k in ks)
                        {
                            if(!ViewData.ContainsKey(k.Key))
                                ViewData.Add(k);
                        }
                        taskid = (string)ViewBag.taskid;
                        keyword =(string) ViewBag.keyword;
                        CStime = (string)ViewBag.CStime;
                        CEtime = (string) ViewBag.CEtime;
                        categoryid = (int)ViewBag.categoryid;
                        nodeid = (int)ViewBag.nodeid;
                        userid = (int)ViewBag.userid;
                        state = (int)ViewBag.state;
                        pagesize = (int)ViewBag.pagesize;
                        pageindex = (int)ViewBag.pageindex;
                    }
                }
                ViewBag.taskid = taskid;
                ViewBag.keyword = keyword;
                ViewBag.CStime = CStime;
                ViewBag.CEtime = CEtime;
                ViewBag.categoryid = categoryid;
                ViewBag.nodeid = nodeid;
                ViewBag.userid = userid;
                ViewBag.state = state;
                ViewBag.pagesize = pagesize;
                ViewBag.pageindex = pageindex;

                HttpContext.Session.SetString(sessionkey, new BSF.Serialization.JsonProvider().Serializer(ViewData));
                #endregion

                tb_webtask_dal dal = new tb_webtask_dal();
                PagedList<tb_webtasklist_model> pageList = null;
                int count = 0;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_webtasklist_model> List = dal.GetList(PubConn, taskid, keyword, CStime, CEtime, categoryid, nodeid, userid, state, pagesize, pageindex, out count);
                    pageList = new PagedList<tb_webtasklist_model>(List, pageindex, pagesize, count);
                    List<tb_node_model> Node = new tb_node_dal().GetListAll(PubConn);
                    List<tb_category_model> Category = new tb_category_dal().GetList(PubConn, "");
                    List<tb_user_model> User = new tb_user_dal().GetAllUsers(PubConn);
                    ViewBag.Node = Node;
                    ViewBag.Category = Category;
                    ViewBag.User = User;
                }
                return View(pageList);
            });
        }

        public ActionResult Add()
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_category_model> Category = new tb_category_dal().GetList(PubConn, "");
                    List<tb_node_model> Node = new tb_node_dal().GetListAll(PubConn);
                    List<tb_user_model> User = new tb_user_dal().GetAllUsers(PubConn);
                    ViewBag.Node = Node;
                    ViewBag.Category = Category;
                    ViewBag.User = User;
                    return View();
                }
            });
        }

        [HttpPost]
        public ActionResult Add( tb_webtask_model model)
        {
            tb_webtask_dal dal = new tb_webtask_dal();
            using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
            {
                PubConn.Open();
                model.taskcreatetime = DateTime.Now;

                int taskid = dal.AddTask(PubConn, model);
            }
            return RedirectToAction("index");
        }
        /// <summary>
        /// webapi 时可以捆绑成一个多个对象的dto
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Update(int taskid)
        {
            using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
            {
                PubConn.Open();
                tb_webtask_dal dal = new tb_webtask_dal();
                tb_webtask_model model = dal.GetOneTask(PubConn, taskid);
                List<tb_category_model> Category = new tb_category_dal().GetList(PubConn, "");
                List<tb_node_model> Node = new tb_node_dal().GetListAll(PubConn);
                List<tb_user_model> User = new tb_user_dal().GetAllUsers(PubConn);
                ViewBag.Node = Node;
                ViewBag.Category = Category;
                ViewBag.User = User;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Update( tb_webtask_model model)
        {
            try
            {
                tb_webtask_dal dal = new tb_webtask_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    var task = dal.GetOneTask(PubConn, model.id);
                    if (task.taskstate == (int)EnumTaskState.Running)
                    {
                        throw new Exception("当前任务在运行中,请停止后提交");
                    }
                    model.taskupdatetime = DateTime.Now;
                    dal.UpdateTask(PubConn, model);
                    return RedirectToAction("index");
                }
            }
            catch (Exception exp)
            {
                ModelState.AddModelError("", exp.Message);
                return View();
            }
        }

        public JsonResult ChangeTaskState(int id, int nodeid, int state)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_command_dal dal = new tb_command_dal();
                tb_webtask_dal taskDal = new tb_webtask_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    if (taskDal.CheckTaskState(PubConn, id) == state)
                    {
                        string msg = state == 1 ? "已开启" : "已关闭";
                        return Json(new { code = -1, msg = msg });
                    }
                    else
                    {
                        tb_command_model m = new tb_command_model()
                        {
                            command = "",
                            commandcreatetime = DateTime.Now,
                            commandname = state == ((int)EnumTaskCommandName.StartWebTask -10)? EnumTaskCommandName.StartWebTask.ToString() : EnumTaskCommandName.StopWebTask.ToString(),
                            taskid = id,
                            nodeid = nodeid,
                            commandstate = (int)EnumTaskCommandState.None
                        };
                        dal.Add(PubConn, m);
                        RedisHelper.SendMessage(new Core.Redis.RedisCommondInfo() { CommondType = Core.Redis.EnumCommondType.TaskCommand, NodeId = m.nodeid });
                    }
                    return Json(new { code = 1, msg = "Success" });
                }

            });
        }

        public JsonResult ChangeMoreTaskState(string poststr)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                List<PostChangeModel> post = new List<PostChangeModel>();
                post = JsonConvert.DeserializeObject<List<PostChangeModel>>(poststr);
                tb_command_dal dal = new tb_command_dal();
                tb_webtask_dal taskDal = new tb_webtask_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    foreach (PostChangeModel m in post)
                    {
                        m.state = m.state == 0 ? 1 : 0;
                        if (taskDal.CheckTaskState(PubConn, m.id) == m.state)
                        {
                            string msg = m.state == 1 ? "已开启" : "已关闭";
                            return Json(new { code = -1, msg = msg });
                        }
                        else
                        {
                            tb_command_model c = new tb_command_model()
                            {
                                command = "",
                                commandcreatetime = DateTime.Now,
                                commandname = m.state == ((int)EnumTaskCommandName.StartWebTask -10)? EnumTaskCommandName.StartWebTask.ToString() : EnumTaskCommandName.StopWebTask.ToString(),
                                taskid = m.id,
                                nodeid = m.nodeid,
                                commandstate = (int)EnumTaskCommandState.None
                            };
                            dal.Add(PubConn, c);
                            RedisHelper.SendMessage(new Core.Redis.RedisCommondInfo() { CommondType = Core.Redis.EnumCommondType.TaskCommand, NodeId = m.nodeid });
                        }
                    }
                    return Json(new { code = 1, data = post });
                }

            });
        }

        public JsonResult CheckTaskState(int id, int state)
        {
            return this.Visit(EnumUserRole.None, () =>
            {              
                tb_webtask_dal dal = new tb_webtask_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    int taskstate = dal.CheckTaskState(PubConn, id);
                    if (taskstate == state)
                    {
                        //TBD  JsonRequestBehavior.AllowGet
                        return Json(new { code = 1, msg = "Success" });
                    }
                    else
                    {
                        //TBD  JsonRequestBehavior.AllowGet
                        return Json(new { code = -1, msg = "" } );
                    }
                }
            });
        }

        public JsonResult Delete(int id)
        {
            try
            {
                tb_webtask_dal dal = new tb_webtask_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    bool state = dal.DeleteOneTask(PubConn, id);
                    return Json(new { code = 1, state = state });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, msg = ex.Message });
            }
        }

        public JsonResult Uninstall(int id)
        {
            try
            {
                tb_command_dal commanddal = new tb_command_dal();
                tb_webtask_dal dal = new tb_webtask_dal();
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    var taskmodel = dal.Get(PubConn, id);
                    dal.UpdateTaskState(PubConn, id, (int)EnumTaskState.Stop);

                    tb_command_model m = new tb_command_model()
                    {
                        command = "",
                        commandcreatetime = DateTime.Now,
                        commandname = EnumTaskCommandName.UninstallTask.ToString(),
                        taskid = id,
                        nodeid = taskmodel.nodeid,
                        commandstate = (int)EnumTaskCommandState.None
                    };
                    commanddal.Add(PubConn, m);
                    RedisHelper.SendMessage(new Core.Redis.RedisCommondInfo() { CommondType = Core.Redis.EnumCommondType.TaskCommand, NodeId = m.nodeid });

                    return Json(new { code = 1 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = -1, msg = ex.Message });
            }
        }
    }
}
