using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NetCoreJwtDemo.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo
{
    public static class JwtTokenMiddleware
    {
        public static IApplicationBuilder UseJwtTokenMiddleware(this IApplicationBuilder app)
        {
            return app.Use(async (ctx, next) =>
            {
                
                if (ctx.User.Identity?.IsAuthenticated != true)
                {
                    var result = await ctx.AuthenticateAsync("JwtBearer");
                    if (result.Succeeded && result.Principal != null)
                    {
                        ctx.User = result.Principal;
                        var token = ctx.Request.Headers["Authorization"].ToString();
                        token = token.Replace("Bearer ", "");
                        var cache=app.ApplicationServices.GetService(typeof(ICache)) as ICache;
                        var existToken = cache.Get<string>("UserToken:"+token);
                        if (string.IsNullOrEmpty(existToken))
                        {
                            ctx.Response.ContentType = "text/html";
                            ctx.Response.StatusCode = 401;
                            await ctx.Response.WriteAsync("");
                            return;
                        }
                    }
                }
                await next();
            });
        }
    }
}
