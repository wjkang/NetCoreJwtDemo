using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Session
{
    public class NullAbpSession : AbpSessionBase
    {
        public static NullAbpSession Instance { get; } = new NullAbpSession();

        public override long? UserId => null;

        public override string UserName => null;
    }
}
