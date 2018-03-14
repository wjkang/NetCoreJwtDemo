using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace NetCoreJwtDemo.Session
{
    public class AspNetCorePrincipalAccessor:DefaultPrincipalAccessor
    {
        public override ClaimsPrincipal Principal
        {
            get
            {
                Console.WriteLine("HttpContextAccessor:" + _httpContextAccessor.GetHashCode());
                Console.WriteLine("HttpContextAccessor.HttpContext:" + _httpContextAccessor.HttpContext.GetHashCode());
                Console.WriteLine("HttpContextAccessor.HttpContext.User:" + _httpContextAccessor.HttpContext.User.GetHashCode());
                return  _httpContextAccessor.HttpContext?.User ?? base.Principal;
            }
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCorePrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            //不管AspNetCorePrincipalAccessor是不是单例，httpContextAccessor都是单例的
            _httpContextAccessor = httpContextAccessor;
            Console.WriteLine("PrincipalAccessor:" + this.GetHashCode().ToString());
        }
    }
}
