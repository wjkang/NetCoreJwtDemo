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

            //1.DataContext根据ContextKey取出数据为空
            //2.ScopeDictionary添加ScopeItem（value 为42，Outer为null),Key为ScopeItem的Id
            //3.DataContext添加key为ContextKey，value为ScopeItem的Id的项
            using (scopeAccessor.BeginScope(ContextKey, new TestData(42)))
            {
                //1.DataContext根据ContextKey取出ScopeItem的Id
                //2.ScopeDictionary根据key取出ScopeItem,返回ScopeItem.value
                var value =scopeAccessor.GetValue(ContextKey);//42

                //1.DataContext根据ContextKey取出外层ScopeItem
                //2.ScopeDictionary添加ScopeItem（value 为42，Outer为外层ScopeItem),Key为ScopeItem的Id
                //3.DataContext添加key为ContextKey，value为新ScopeItem的Id的项
                using (scopeAccessor.BeginScope(ContextKey, new TestData(24)))
                {
                    //1.DataContext根据ContextKey取出当前域的ScopeItem的Id
                    //2.ScopeDictionary根据key取出当前域ScopeItem,返回当前域ScopeItem.value
                    var value1 =scopeAccessor.GetValue(ContextKey);//24
                }
                //1.调用dispose
                //2.ScopeDictionary移除当前域ScopeItem
                //3.如果存在上层域，DataContext设置key为ContextKey的value为当前域ScopeItem的outer.Id



                var value2 =scopeAccessor.GetValue(ContextKey);//42
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
