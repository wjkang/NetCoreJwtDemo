using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreJwtDemo.Session;

namespace NetCoreJwtDemo.Controllers
{

    [Route("[controller]")]
    public class UserController : Controller
    {

        private IAbpSession session;

        public UserController(IAbpSession session)
        {
            this.session = session;
        }

        [Authorize]
        [Route("info")]
        [HttpGet]
        public IActionResult Info()
        {
            var userId = session.UserId;
            return new ObjectResult(new ResponseData()
            {
                StatusCode = 200,
                Msg = "",
                Data = new UserInfo()
                {
                    UserId = userId ?? 0,
                    UserName = "Admin123",
                    Roles=new string[]
                    {
                        "admin"
                    },
                    FunctionCodes=new string[]
                    {
                        "function"
                    }
                }
            });

        }
        private class UserInfo
        {
            public long UserId { get; set; }
            public string UserName { get; set; }

            public  string[] Roles { get; set; }

            public string[] FunctionCodes { get; set; }
        }
    }


}