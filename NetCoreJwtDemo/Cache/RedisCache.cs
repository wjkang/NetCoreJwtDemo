using Microsoft.Extensions.Options;
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
        private volatile ConnectionMultiplexer _connection;
        private IDatabase _cache;

        private readonly RedisCacheOptions _options;
        private readonly string _instance;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);


        public RedisCache(IOptions<RedisCacheOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;

            _instance = _options.InstanceName ?? string.Empty;
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
