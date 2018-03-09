using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Cache
{
    public class RedisCache : ICache, IDisposable
    {
        int Default_Timeout = 600;//默认超时时间（单位秒）
        private volatile ConnectionMultiplexer _connection;
        private IDatabase _cache;

        private readonly RedisCacheOptions _options;
        private readonly string _instance;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        JsonSerializerSettings jsonConfig = new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };



        public RedisCache(IOptions<RedisCacheOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;

            if(string.IsNullOrEmpty(_options.InstanceName))
            {
                throw new ArgumentNullException(nameof(optionsAccessor.Value.InstanceName));
            }
            _instance = _options.InstanceName;
        }

        class CacheObject<T>
        {
            public int ExpireTime { get; set; }
            public bool ForceOutofDate { get; set; }
            public T Value { get; set; }
        }

        // <summary>
        /// 连接超时设置
        /// </summary>
        public int TimeOut
        {
            get
            {
                return Default_Timeout;
            }
            set
            {
                Default_Timeout = value;
            }
        }


        public object Get(string key)
        {
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            Connect();
            DateTime begin = DateTime.Now;
            var cacheValue = _cache.StringGet(GetKeyForRedis(key));
            DateTime endCache = DateTime.Now;
            var value = default(T);
            if (!cacheValue.IsNull)
            {
                var cacheObject = JsonConvert.DeserializeObject<CacheObject<T>>(cacheValue, jsonConfig);
                if (!cacheObject.ForceOutofDate)
                    _cache.KeyExpire(GetKeyForRedis(key), new TimeSpan(0, 0, cacheObject.ExpireTime));
                value = cacheObject.Value;
            }
            DateTime endJson = DateTime.Now;
            return value;
        }

        public void Remove(string key)
        {
            Connect();
            _cache.KeyDelete(GetKeyForRedis(key), CommandFlags.HighPriority);
        }

        public void Insert(string key, object data)
        {
            Connect();
            var jsonData = GetJsonData(data,-1, false);
            _cache.StringSet(GetKeyForRedis(key), jsonData);
        }

        public void Insert<T>(string key, T data)
        {
            Connect();
            var jsonData = GetJsonData<T>(data, -1, false);
            _cache.StringSet(GetKeyForRedis(key), jsonData);
        }

        public void Insert(string key, object data, int cacheTime)
        {
            Connect();
            var timeSpan = TimeSpan.FromSeconds(cacheTime);
            var jsonData = GetJsonData(data, cacheTime, true);
            _cache.StringSet(GetKeyForRedis(key), jsonData, timeSpan);
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            Connect();
            var timeSpan = TimeSpan.FromSeconds(cacheTime);
            var jsonData = GetJsonData<T>(data, cacheTime, true);
            _cache.StringSet(GetKeyForRedis(key), jsonData, timeSpan);
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            Connect();
            var timeSpan = cacheTime - DateTime.Now;
            var jsonData = GetJsonData(data,(int)timeSpan.TotalSeconds, true);
            _cache.StringSet(GetKeyForRedis(key), jsonData, timeSpan);
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            Connect();
            var timeSpan = cacheTime - DateTime.Now;
            var jsonData = GetJsonData<T>(data, (int)timeSpan.TotalSeconds, true);
            _cache.StringSet(GetKeyForRedis(key), jsonData, timeSpan);
        }

        public bool Exists(string key)
        {
            return _cache.KeyExists(GetKeyForRedis(key));
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        private void Connect()
        {
            if (_connection != null)
            {
                return;
            }

            _connectionLock.Wait();
            try
            {
                if (_connection == null)
                {
                    _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
        private string GetKeyForRedis(string key)
        {
            return _instance +":"+key;
        }
        private string GetJsonData(object data, int cacheTime, bool forceOutOfDate)
        {
            var cacheObject = new CacheObject<object>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate };
            return JsonConvert.SerializeObject(cacheObject, jsonConfig);//序列化对象
        }
        private string GetJsonData<T>(T data, int cacheTime, bool forceOutOfDate)
        {
            var cacheObject = new CacheObject<T>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate };
            return JsonConvert.SerializeObject(cacheObject, jsonConfig);//序列化对象
        }
    }
}
