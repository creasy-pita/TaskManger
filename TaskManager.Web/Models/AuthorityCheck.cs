using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class AuthorityCheck : AuthorizeAttribute
    {
        //TBD
        //protected override bool AuthorizeCore(HttpContext httpContext)
        //{
        //    try
        //    {
        //        var userlogininfo = UserLoginInfo.CurrentUserLoginInfo;
        //        if (userlogininfo != null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception exp)
        //    {
        //        return false;
        //    }
        //}
    }
}