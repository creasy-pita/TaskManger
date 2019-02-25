
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.Core;

namespace TaskManager.Web.Models
{
    public class BaseWebController : Controller
    {
        /// <summary>
        /// web 访问控制器
        /// 错误拦截
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ActionResult Visit(EnumUserRole role,Func<ActionResult> action)
        {
            return this.Visit<ActionResult>(role, action);
        }

        /// <summary>
        /// web 访问控制器
        /// 错误拦截
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public T Visit<T>(EnumUserRole userrole, Func<T> action)
        {
            try
            {
                int currentuserrole = -2;
                if (UserLoginInfo.CurrentUserLoginInfo != null && UserLoginInfo.CurrentUserLoginInfo.UserModel != null)
                {
                    currentuserrole = UserLoginInfo.CurrentUserLoginInfo.UserModel.userrole;
                }
                string Number = UserLoginInfo.CurrentUserLoginInfo.UserModel.userstaffno;
                if ((int)userrole == (int)currentuserrole || (int)userrole == -1)
                {
                    ViewBag.Role = currentuserrole;
                    ViewBag.Number = Number;
                    return action.Invoke();
                }
                else
                {
                    throw new Exception("无权访问！");
                }
            }
            catch (Exception exp)
            {
                //异常返回
                throw exp;
            }
        }

       



    }
}