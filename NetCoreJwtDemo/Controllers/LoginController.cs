using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using NetCoreJwtDemo.Cache;
using Microsoft.AspNetCore.Authorization;

namespace NetCoreJwtDemo.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private IConfigurationRoot _appConfiguration;
        private ICache _cache;

        public LoginController(IConfigurationRoot configurationRoot, ICache cache)
        {
            _appConfiguration = configurationRoot;
            _cache = cache;
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                return new ObjectResult(new ResponseData()
                {
                    StatusCode = 200,
                    Msg = "",
                    Data = new Token()
                    {
                        AccessToken = GenerateToken(username),
                        RefreshToken = ""
                    }
                });
            }

            return BadRequest();
        }

        [Route("logout")]
        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            _cache.Remove("UserToken:" + token.Replace("Bearer ", ""));
            return new ObjectResult(new ResponseData()
            {
                StatusCode = 200,
                Msg = "",
                Data = null
            });
        }

        private string GenerateToken(string username)
        {
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.NameIdentifier,"1")
            };
            var now = DateTime.UtcNow;
            var tokenAuthConfig = GetTokenAuthConfiguration();
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: tokenAuthConfig.Issuer,
                audience: tokenAuthConfig.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(tokenAuthConfig.Expiration),
                signingCredentials: tokenAuthConfig.SigningCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            _cache.Insert("UserToken:" + token, username, (int)tokenAuthConfig.Expiration.TotalSeconds);
            return token;
        }
        private TokenAuthConfiguration GetTokenAuthConfiguration()
        {
            var tokenAuthConfig = new TokenAuthConfiguration();
            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(30);
            return tokenAuthConfig;
        }
    }
}