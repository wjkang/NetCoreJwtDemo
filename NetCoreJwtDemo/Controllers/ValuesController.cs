using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NetCoreJwtDemo.Cache;

namespace NetCoreJwtDemo.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private ICache _cache;

        public ValuesController(ICache cache)
        {
            _cache = cache;
        }
        // GET api/values
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var claims = HttpContext.User.Claims;
            var token = _cache.Get<string>("UserToken:" + 1);
            return new string[] {token };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";   
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
