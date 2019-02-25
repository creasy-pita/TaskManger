using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManager.Domain.Model;

namespace TaskManager.Web.Models
{
    [Serializable]
    public class UserLoginInfo
    {
        /// <summary>
        /// 数据库中的工号，web.config中的用户名
        /// </summary>
        public string UserName { get; set; }

        public tb_user_model UserModel { get; set; }
        public static UserLoginInfo CurrentUserLoginInfo
        { get; set; }

        //TBD
        //public static UserLoginInfo CurrentUserLoginInfo { get {
        //    return System.Web.HttpContext.Current.Session["UserLoginInfo"] == null ? null : System.Web.HttpContext.Current.Session["UserLoginInfo"] as UserLoginInfo;
        //} set {
        //    System.Web.HttpContext.Current.Session["UserLoginInfo"] = value;
        //}}
    }
}