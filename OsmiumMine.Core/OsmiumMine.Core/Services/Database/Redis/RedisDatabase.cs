using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmiumMine.Core.Services.Database.Redis
{
    public class RedisDatabase : IKeyValueDatabase, IDisposable
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _rdb => _redis.GetDatabase();

        public RedisDatabase(StackExchange.Redis.ConfigurationOptions connectionConfig)
        {
            _redis = ConnectionMultiplexer.Connect(connectionConfig);
        }

        public bool Exists(string key)
        {
            return _rdb.KeyExists(key);
        }

        public string Get(string key)
        {
            return _rdb.StringGet(key);
        }

        public string Get(string domain, string key)
        {
            return _rdb.HashGet(domain, key);
        }

        public bool Set(string key, string value)
        {
            return _rdb.StringSet(key, value);
        }

        public bool Set(string domain, string key, string value)
        {
            return _rdb.HashSet(domain, key, value);
        }

        public void Dispose()
        {
            _redis.Close();
        }

        public string[] GetKeys(string domain)
        {
            return GetKeysEnumerable(domain).ToArray();
        }

        public IEnumerable<string> GetKeysEnumerable(string domain)
        {
            return _rdb.HashKeys(domain).Select(x => x.ToString());
        }

        public bool Delete(string key)
        {
            return _rdb.KeyDelete(key);
        }

        public bool Delete(string domain, string key)
        {
            return _rdb.HashDelete(domain, key);
        }
    }
}