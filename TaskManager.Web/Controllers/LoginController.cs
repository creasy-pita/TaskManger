using TaskManager.Domain.Dal;
using TaskManager.Domain.Model;
using TaskManager.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using BSF.Extensions;
using BSF.Config;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Runtime.CompilerServices;

namespace TaskManager.Web.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        //登录
        [HttpGet]
        public ActionResult Login(string appid, string sign, string returnurl)
        {
            var userlogininfo = UserLoginInfo.CurrentUserLoginInfo;
            if (userlogininfo != null)
            {
                return RedirectToAction("index", "Task");
            }
            return View();
        }

        //登录
        [HttpPost]
        public ActionResult Login(string appid, string sign, string returnurl, string username, string password, string validate)
        {
            try
            {
                returnurl = returnurl ?? "";
                username = username ?? "";
                password = password ?? "";
                validate = validate ?? "";
                ViewBag.username = username;

                if (ConfigHelper.Configuration.GetValue<string>("LoginUser").Contains(";" + username.NullToEmpty() + "," + password.NullToEmpty() + ";"))
                {
                    UserLoginInfo.CurrentUserLoginInfo = new UserLoginInfo() { UserName = username.NullToEmpty() };
                }

                if (UserLoginInfo.CurrentUserLoginInfo != null)
                {
                    #region 6写auth Cookie
                    tb_user_model user = Common.GetUserName(username);
                    if (user == null)
                        throw new Exception("用户已登陆,但该用户未在平台中开权限，请联系管理员添加。");

                    UserLoginInfo.CurrentUserLoginInfo.UserModel = user;

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.username),
                        };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                #endregion
                return RedirectToAction("index", "Task", new { userid = user.id });
                }
                else
                {
                    ModelState.AddModelError("", "用户未登陆成功，请联系管理员在web.config中配置用户，并在平台后台开启权限");
                    return View();
                }
            }
            catch (Exception exp)
            {
                ModelState.AddModelError("", "登陆出错,请咨询管理员。错误信息:"+exp.Message);
                return View();
            }
        }

        //登出
        public ActionResult Logout(string returnurl)
        {
            UserLoginInfo.CurrentUserLoginInfo = null;
            TaskAwaiter tw =HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter();
            tw.GetResult();
            if (string.IsNullOrEmpty(returnurl))
                return RedirectToAction("Login");
            else
                return Redirect(returnurl);
        }

      
    }
}
