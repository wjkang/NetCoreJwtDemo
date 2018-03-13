using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Remoting
{
    public class AsyncLocalAmbientDataContext : IAmbientDataContext
    {
        //AsyncLocal（区别ThreadLocal）避免异步上下文的切换过程中有可能会导致数据丢失
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> AsyncLocalDictionary = new ConcurrentDictionary<string, AsyncLocal<object>>();

        public void SetData(string key, object value)
        {
            var asyncLocal = AsyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object>());
            asyncLocal.Value = value;
        }

        public object GetData(string key)
        {
            var asyncLocal = AsyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object>());
            return asyncLocal.Value;
        }
    }
}
