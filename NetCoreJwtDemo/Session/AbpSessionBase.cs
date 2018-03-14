using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Session
{
    public abstract class AbpSessionBase : IAbpSession
    {
        public abstract long? UserId { get; }

        public abstract string UserName { get; }

        public IDisposable Use(long? userId)
        {
            throw new NotImplementedException();
        }
    }
}
