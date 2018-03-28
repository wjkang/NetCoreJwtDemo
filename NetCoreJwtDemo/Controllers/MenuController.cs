using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreJwtDemo.Controllers
{
    [Route("[controller]")]
    public class MenuController : Controller
    {
        [Authorize]
        [Route("getaccessmenu")]
        [HttpGet]
        public IActionResult GetAccessMenu()
        {
            return new ObjectResult(new ResponseData()
            {
                StatusCode = 200,
                Msg = "系统错误",
                Data = new List<Menu>()
                {
                    new Menu()
                    {
                        Path="/",
                        Icon="settings",
                        Title="首页",
                        Name="otherRouter",
                        leftMemu=false,
                        Children=new List<Menu>()
                        {
                            new Menu()
                            {
                                Path="home",
                                Title="首页",
                                Name="home_index"
                            },
                            new Menu()
                            {
                                Path="userinfo",
                                Title="个人中心",
                                Name="userinfo"
                            }
                        }
                    },
                    new Menu()
                    {
                        Path="/system",
                        Icon="settings",
                        Title="系统设置",
                        Name="系统设置",
                        leftMemu=true,
                        Children=new List<Menu>()
                        {
                            new Menu()
                            {
                                Path="menu",
                                Icon="android-menu",
                                Title="菜单管理",
                                Name="menu"
                            }
                        }
                    },
                    new Menu()
                    {
                        Path="/permission",
                        Icon="key",
                        Title="权限管理",
                        Name="权限管理",
                        leftMemu=true,
                        Children=new List<Menu>()
                        {
                            new Menu()
                            {
                                Path="function",
                                Icon="wrench",
                                Title="功能管理",
                                Name="function"
                            },
                            new Menu()
                            {
                                Path="role",
                                Icon="ios-personadd-outline",
                                Title="角色管理",
                                Name="role"
                            },
                            new Menu()
                            {
                                Path="rolepermission",
                                Icon="settings",
                                Title="角色权限管理",
                                Name="rolepermission"
                            }
                        }
                    },
                    new Menu()
                    {
                        Path="/user",
                        Icon="ios-people",
                        Title="用户管理",
                        Name="用户管理",
                        leftMemu=true,
                        Children=new List<Menu>()
                        {
                            new Menu()
                            {
                                Path="index",
                                Icon="ios-people",
                                Title="用户管理",
                                Name="user_index"
                            }
                        }
                    }
                }
            });
        }

        private class Menu
        {
            public string Path { get; set; }

            public string Icon { get; set; }

            public string Title { get; set; }

            public string Name { get; set; }

            public bool leftMemu { get; set; }

            public IList<Menu> Children { get; set; }
        }
    }
}