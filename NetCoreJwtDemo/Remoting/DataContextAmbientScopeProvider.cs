using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Remoting
{
    public class DataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
    {
        private class ScopeItem
        {
            public string Id { get; }

            /// <summary>
            /// 外层数据
            /// </summary>
            public ScopeItem Outer { get; }

            public T Value { get; }

            public ScopeItem(T value, ScopeItem outer = null)
            {
                Id = Guid.NewGuid().ToString();

                Value = value;
                Outer = outer;
            }
        }

        private static readonly ConcurrentDictionary<string, ScopeItem> ScopeDictionary = new ConcurrentDictionary<string, ScopeItem>();

        private readonly IAmbientDataContext _dataContext;

        public DataContextAmbientScopeProvider(IAmbientDataContext dataContext)
        {

            _dataContext = dataContext;

        }

        public IDisposable BeginScope(string contextKey, T value)
        {
            var item = new ScopeItem(value, GetCurrentItem(contextKey));

            if (!ScopeDictionary.TryAdd(item.Id, item))
            {
                throw new Exception("Can not add item! ScopeDictionary.TryAdd returns false!");
            }

            _dataContext.SetData(contextKey, item.Id);

            return new DisposeAction(() =>
            {
                ScopeDictionary.TryRemove(item.Id, out item);

                if (item.Outer == null)
                {
                    _dataContext.SetData(contextKey, null);
                    return;
                }

                _dataContext.SetData(contextKey, item.Outer.Id);
            });
        }

        public T GetValue(string contextKey)
        {
            var item = GetCurrentItem(contextKey);
            if (item == null)
            {
                return default(T);
            }

            return item.Value;
        }

        private ScopeItem GetCurrentItem(string contextKey)
        {
            var objKey = _dataContext.GetData(contextKey) as string;

            ScopeItem obj;
            
            return objKey != null ?(ScopeDictionary.TryGetValue(objKey, out obj) ? obj : default(ScopeItem)) : null;
        }


        private void Test()
        {

            var ContextKey = "Abp.Tests.TestData";
            var scopeAccessor = new DataContextAmbientScopeProvider<TestData>(
                new AsyncLocalAmbientDataContext()
            );

            //scopeAccessor.GetValue(ContextKey).ShouldBeNull();

            using (scopeAccessor.BeginScope(ContextKey, new TestData(42)))
            {
                //scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);

                using (scopeAccessor.BeginScope(ContextKey, new TestData(24)))
                {
                    //scopeAccessor.GetValue(ContextKey).Number.ShouldBe(24);
                }

                //scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);
            }

            //scopeAccessor.GetValue(ContextKey).ShouldBeNull();
        }
        private class TestData
        {
            public TestData(int number)
            {
                Number = number;
            }

            public int Number { get; set; }
        }
    }
}
