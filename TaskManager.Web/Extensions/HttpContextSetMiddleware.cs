using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Extensions
{
    public class HttpContextSetMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpContextSetMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            // Do something with context near the beginning of request processing.
            System.Web.HttpContext.Configure(context);
            Console.WriteLine($"QueryString{context.Request.Path + context.Request.QueryString}");
            await _next.Invoke(context);
            // Clean up.
        }
    }

    public static class HttpContextSetExtension
    {
        public static IApplicationBuilder UseHttpContextSetMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpContextSetMiddleware>();

        }

    }
}
