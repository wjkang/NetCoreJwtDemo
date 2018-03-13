using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Remoting
{
    public class DisposeAction : IDisposable
    {
        private Action _action;

        public DisposeAction( Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            // Interlocked prevents multiple execution of the _action.
            var action = Interlocked.Exchange(ref _action, null);
            action?.Invoke();
        }
    }
}
