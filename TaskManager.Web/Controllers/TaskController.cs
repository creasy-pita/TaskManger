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
    public class TaskController : BaseWebController
    {
        //
        // GET: /Task/

        public ActionResult Index(string taskid, string keyword, string CStime, string CEtime, int categoryid = -1, int nodeid = -1, int userid = -1, int state = -999, int pagesize = 10, int pageindex = 1)
        {
            return this.Visit(EnumUserRole.None, () =>
            {
                #region 保存查询信息,优化操作体验
                var ps = Request.RequestParams();
                ps.Remove("userid");
                var sessionkey = "/task/index/";
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

                tb_task_dal dal = new tb_task_dal();
                PagedList<tb_tasklist_model> pageList = null;
                int count = 0;
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    List<tb_tasklist_model> List = dal.GetList(PubConn, taskid, keyword, CStime, CEtime, categoryid, nodeid, userid, state, pagesize, pageindex, out count);
                    pageList = new PagedList<tb_tasklist_model>(List, pageindex, pagesize, count);
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
        public ActionResult Add(IFormFile TaskDll,  tb_task_model model, string tempdatajson)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                string filename = TaskDll.FileName;
                byte[] dllbyte;
                using (var dll = TaskDll.OpenReadStream())
                {
                    dllbyte = new byte[dll.Length];
                    dll.Read(dllbyte, 0, Convert.ToInt32(dll.Length));
                }

                tb_task_dal dal = new tb_task_dal();
                tb_version_dal dalversion = new tb_version_dal();
                tb_tempdata_dal tempdatadal = new tb_tempdata_dal();
                //model.taskcreateuserid = Common.GetUserId(this);
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    model.taskcreatetime = DateTime.Now;
                    model.taskversion = 1;
                    int taskid = dal.AddTask(PubConn, model);
                    dalversion.Add(PubConn, new tb_version_model()
                    {
                        taskid = taskid,
                        version = 1,
                        versioncreatetime = DateTime.Now,
                        zipfile = dllbyte,
                        zipfilename = System.IO.Path.GetFileName(filename)
                    });
                    tempdatadal.Add(PubConn, new tb_tempdata_model()
                    {
                        taskid = taskid,
                        tempdatajson = tempdatajson,
                        tempdatalastupdatetime = DateTime.Now
                    });
                }
                return RedirectToAction("index");
            });
        }

        [HttpPost]
        public ActionResult AddFullInfo2([FromBody]SimpleModel info)
        {
            return Ok(info);
        }

        /// <summary>
        /// 添加完整任务 并一次性保存
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddFullInfo([ModelBinder(Name = "json")][FromBody]FullTaskInfo info)
        {
            //TBD 保存失败时返回 code=-1
           // return info.config_models[0].filecontent;
            IFormFile TaskDll = info.TaskDll;
            tb_task_model model = info.model;
            tb_task_config_model[] config_models = info.config_models;
            string tempdatajson = info.tempdatajson;

            string filename = TaskDll.FileName;
            byte[] dllbyte;
            using (var dll = TaskDll.OpenReadStream())
            {
                dllbyte = new byte[dll.Length];
                dll.Read(dllbyte, 0, Convert.ToInt32(dll.Length));
            }

            tb_task_dal dal = new tb_task_dal();
            tb_task_config_dal config_dal = new tb_task_config_dal();
            tb_version_dal dalversion = new tb_version_dal();
            tb_tempdata_dal tempdatadal = new tb_tempdata_dal();
            //model.taskcreateuserid = Common.GetUserId(this);
            using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
            {
                PubConn.Open();
                PubConn.BeginTransaction();
                try
                {
                    model.taskcreatetime = DateTime.Now;
                    model.taskversion = 1;
                    int taskid = dal.AddTask(PubConn, model);
                    dalversion.Add(PubConn, new tb_version_model()
                    {
                        taskid = taskid,
                        version = 1,
                        versioncreatetime = DateTime.Now,
                        zipfile = dllbyte,
                        zipfilename = System.IO.Path.GetFileName(filename)
                    });
                    foreach (var config_Model in config_models)
                    {
                        config_dal.Add(PubConn, new tb_task_config_model()
                        {
                            filecontent = config_Model.filecontent,
                            filename = config_Model.filename,
                            relativePath = config_Model.relativePath,
                            taskid = taskid,
                            lastupdatetime = DateTime.Now
                        });
                    }

                    tempdatadal.Add(PubConn, new tb_tempdata_model()
                    {
                        taskid = taskid,
                        tempdatajson = tempdatajson,
                        tempdatalastupdatetime = DateTime.Now
                    });
                    PubConn.Commit();
                }
                catch (Exception ex)
                {
                    PubConn.Rollback();
                }
                finally { }
            }
            return Json(new { code = 1, msg = "" } );
            //return RedirectToAction("index");
            //return info.config_models[0].filecontent;
        }
        public ActionResult Update(int taskid)
        {
            return this.Visit(EnumUserRole.None, () =>
            {
                using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                {
                    PubConn.Open();
                    tb_task_dal dal = new tb_task_dal();
                    tb_task_model model = dal.GetOneTask(PubConn, taskid);
                    tb_tempdata_model tempdatamodel = new tb_tempdata_dal().GetByTaskID(PubConn, taskid);
                    List<tb_version_model> Version = new tb_version_dal().GetTaskVersion(PubConn, taskid);
                    List<tb_category_model> Category = new tb_category_dal().GetList(PubConn, "");
                    List<tb_node_model> Node = new tb_node_dal().GetListAll(PubConn);
                    List<tb_user_model> User = new tb_user_dal().GetAllUsers(PubConn);
                    ViewBag.Node = Node;
                    ViewBag.Category = Category;
                    ViewBag.Version = Version;
                    ViewBag.User = User;
                    ViewBag.TempData = tempdatamodel;
                    return View(model);
                }
            });
        }

        [HttpPost]
        public ActionResult Update(IFormFile TaskDll, tb_task_model model, string tempdatajson)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                try
                {
                    tb_task_dal dal = new tb_task_dal();
                    tb_version_dal dalversion = new tb_version_dal();
                    tb_tempdata_dal tempdatadal = new tb_tempdata_dal();
                    byte[] dllbyte = null;
                    string filename = "";
                    int change = model.taskversion;
                    if (change == -1)
                    {
                        if (TaskDll == null)
                        {
                            throw new Exception("没有文件！");
                        }
                        filename = TaskDll.FileName;
                        using (var dll = TaskDll.OpenReadStream())
                        {
                            dllbyte = new byte[dll.Length];
                            dll.Read(dllbyte, 0, Convert.ToInt32(dll.Length));
                        }
                        //model.taskcreateuserid = Common.GetUserId(this);
                    }
                    using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                    {
                        PubConn.Open();
                        var task = dal.GetOneTask(PubConn, model.id);
                        if (task.taskstate == (int)EnumTaskState.Running)
                        {

                            throw new Exception("当前任务在运行中,请停止后提交");
                        }
                        if (change == -1)
                        {
                            model.taskversion = dalversion.GetVersion(PubConn, model.id) + 1;
                        }
                        model.taskupdatetime = DateTime.Now;
                        dal.UpdateTask(PubConn, model);
                        if (change == -1)
                        {
                            dalversion.Add(PubConn, new tb_version_model()
                            {
                                taskid = model.id,
                                version = model.taskversion,
                                versioncreatetime = DateTime.Now,
                                zipfile = dllbyte,
                                zipfilename = System.IO.Path.GetFileName(filename)
                            });
                        }
                        tempdatadal.UpdateByTaskID(PubConn, new tb_tempdata_model()
                        {
                            taskid = model.id,
                            tempdatajson = tempdatajson,
                            tempdatalastupdatetime = DateTime.Now
                        });
                        return RedirectToAction("index");
                    }
                }
                catch (Exception exp)
                {
                    ModelState.AddModelError("", exp.Message);
                    return View();
                }
            });
        }

        public JsonResult ChangeTaskState(int id, int nodeid, int state)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                tb_command_dal dal = new tb_command_dal();
                tb_task_dal taskDal = new tb_task_dal();
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
                            commandname = state == (int)EnumTaskCommandName.StartTask ? EnumTaskCommandName.StartTask.ToString() : EnumTaskCommandName.StopTask.ToString(),
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
                tb_task_dal taskDal = new tb_task_dal();
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
                                commandname = m.state == (int)EnumTaskCommandName.StartTask ? EnumTaskCommandName.StartTask.ToString() : EnumTaskCommandName.StopTask.ToString(),
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
                tb_task_dal dal = new tb_task_dal();
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
            return this.Visit(EnumUserRole.Admin, () =>
            {
                try
                {
                    tb_task_dal dal = new tb_task_dal();
                    tb_task_config_dal task_Config_Dal = new tb_task_config_dal();
                    using (DbConn PubConn = DbConn.CreateConn(Config.TaskConnectString))
                    {
                        PubConn.Open();
                        PubConn.BeginTransaction();
                        bool state =false;
                        try
                        {
                            state = dal.DeleteOneTask(PubConn, id);
                            List<tb_task_config_model> task_config_models = task_Config_Dal.GetList(PubConn, id);
                            foreach (var  task_config_model in task_config_models)
                            {
                                state = task_Config_Dal.Delete(PubConn, task_config_model.id);
                            }
                            PubConn.Commit();
                        }
                        catch (Exception ex)
                        {
                            PubConn.Rollback();
                        }

                        return Json(new { code = 1, state = state });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { code = -1, msg = ex.Message });
                }
            });
        }

        public JsonResult Uninstall(int id)
        {
            return this.Visit(EnumUserRole.Admin, () =>
            {
                try
                {
                    tb_command_dal commanddal = new tb_command_dal();
                    tb_task_dal dal = new tb_task_dal();
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
            });
        }

        public ActionResult Cron()
        {
            return View();
        }

        public ActionResult CustomCorn()
        {
            return View();
        }
        public string Test()
        {
            string conn = ConfigHelper.Configuration.GetValue<string>("TaskConnectString");
            SqlConnection con = new SqlConnection(conn);
            if (con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "select * from tb_user";
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable da = new DataTable();
                    adapter.Fill(da);
                    return da.Rows.Count.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return null;
        }
    }
}
