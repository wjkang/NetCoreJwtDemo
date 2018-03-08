using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Cache
{
    public class RedisCacheOptions : IOptions<RedisCacheOptions>
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; }

        RedisCacheOptions IOptions<RedisCacheOptions>.Value
        {
            get { return this; }
        }
    }
}
