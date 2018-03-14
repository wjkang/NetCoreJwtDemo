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

namespace NetCoreJwtDemo.Controllers
{
    [Route("[controller]")]
    public class TokenController : Controller
    {
        private IConfigurationRoot _appConfiguration;
        private ICache _cache;

        public TokenController(IConfigurationRoot configurationRoot,ICache cache)
        {
            _appConfiguration = configurationRoot;
            _cache = cache;
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(string username, string password)
        {
            
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                return new ObjectResult(GenerateToken(username));
            }
            
            return BadRequest();
        }



        private string GenerateToken(string username)
        {
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name,username),
                new Claim("UserId","1")
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
            var token= new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            _cache.Insert("UserToken:"+token, username,(int)tokenAuthConfig.Expiration.TotalSeconds);
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