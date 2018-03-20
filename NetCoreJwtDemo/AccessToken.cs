using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo
{
    public class Token
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
