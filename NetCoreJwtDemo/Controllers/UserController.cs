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
                    UserName = "Admin345",
                    Roles=new string[]
                    {
                        "admin"
                    },
                    FunctionCodes=new string[]
                    {
                        "function",
                        "role",
                        "rolepermission",
                        "roleuser",
                        "userrole"
                    }
                }
            });

        }

        [Authorize]
        [Route("userlist")]
        [HttpGet]
        public IActionResult UserList()
        {
            return new ObjectResult(new ResponseData()
            {
                StatusCode = 200,
                Msg = "",
                Data = new List<UserEntity>()
                {
                    new UserEntity()
                    {
                        Id=1,
                        LoginName="test1",
                        TrueName="张三",
                        Email="12ewewewe@sina.com"
                    },
                    new UserEntity()
                    {
                        Id=2,
                        LoginName="test2",
                        TrueName="李四",
                        Email="3434343434@qq.com"
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

        public class UserEntity
        {
            public long Id { get; set; }

            public string LoginName { get; set; }

            public string TrueName { get; set; }

            public string Email { get; set; }
        }
    }


}