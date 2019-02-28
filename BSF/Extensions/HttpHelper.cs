using System;
using System.Collections.Generic;
using System.Text;

namespace System.Web
{

    public static class HttpContext
    {
        public static void Configure(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            Current = httpContext;
        }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get;
            set;
        }
    }
}
