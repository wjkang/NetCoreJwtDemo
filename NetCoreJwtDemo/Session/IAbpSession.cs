using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Session
{
    public interface IAbpSession
    {
        long? UserId { get; }

        string UserName { get;}

        IDisposable Use(long? userId);
    }
}
