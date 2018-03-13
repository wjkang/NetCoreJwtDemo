using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Remoting
{
    /// <summary>
    /// 上下文数据
    /// </summary>
    public interface IAmbientDataContext
    {
        void SetData(string key, object value);

        object GetData(string key);
    }
}
