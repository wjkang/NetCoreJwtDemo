using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Session
{
    public abstract class AbpSessionBase : IAbpSession
    {
        public abstract long? UserId { get; }

        public IDisposable Use(int? tenantId, long? userId)
        {
            throw new NotImplementedException();
        }
    }
}
