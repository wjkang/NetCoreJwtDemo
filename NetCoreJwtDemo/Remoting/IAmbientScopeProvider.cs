using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Remoting
{
    /// <summary>
    /// 环绕作用域接口
    /// </summary>
    interface IAmbientScopeProvider<T>
    {
        T GetValue(string contextKey);

        IDisposable BeginScope(string contextKey, T value);
    }
}
