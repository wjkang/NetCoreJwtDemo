using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo
{
    public class ResponseData
    {
        public int StatusCode { get; set; }

        public string Msg { get; set; }

        public object Data { get; set; }
    }
}
