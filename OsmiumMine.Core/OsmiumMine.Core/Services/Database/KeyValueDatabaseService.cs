using System.Collections.Generic;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Services.Database.Redis;
using StackExchange.Redis;

namespace OsmiumMine.Core.Services.Database
{
    public class KeyValueDatabaseService
    {
        public KeyValueDatabaseService(OsmiumMineContext context)
        {
            Context = context;

            var commandMap = CommandMap.Default;
            if (Context.Configuration.DatabaseType != OMDatabaseConfiguration.RedisDatabaseType.Redis)
                switch (Context.Configuration.DatabaseType)
                {
                    case OMDatabaseConfiguration.RedisDatabaseType.SSDB:
                        commandMap = CommandMap.Create(new HashSet<string>
                        {
                            // see http://www.ideawu.com/ssdb/docs/redis-to-ssdb.html
                            "ping",
                            "get",
                            "set",
                            "del",
                            "incr",
                            "incrby",
                            "mget",
                            "mset",
                            "keys",
                            "getset",
                            "setnx",
                            "hget",
                            "hset",
                            "hdel",
                            "hincrby",
                            "hkeys",
                            "hvals",
                            "hmget",
                            "hmset",
                            "hlen",
                            "zscore",
                            "zadd",
                            "zrem",
                            "zrange",
                            "zrangebyscore",
                            "zincrby",
                            "zdecrby",
                            "zcard",
                            "llen",
                            "lpush",
                            "rpush",
                            "lpop",
                            "rpop",
                            "lrange",
                            "lindex",
                            "exists"
                        }, true);
                        break;
                }

            Store = new RedisDatabase(new ConfigurationOptions
            {
                EndPoints = {{Context.Configuration.RedisAddress, Context.Configuration.RedisPort}},
                CommandMap = commandMap
            });
        }

        public OsmiumMineContext Context { get; set; }
        public IKeyValueDatabase Store { get; }

        /// <summary>
        ///     Gets the data store prefix key
        /// </summary>
        /// <param name="subDatabaseId"></param>
        /// <returns></returns>
        public string GetDSPrefixKey(string subDatabaseId, bool includeSuffix = true)
        {
            return $"{Context.Configuration.RedisPrefix + subDatabaseId}" + (includeSuffix ? "." : "");
        }

        public string GetDomainPath(string domain)
        {
            return $"{Context.Configuration.RedisPrefix + domain}";
        }

        public string GetDomainSuffix(string domain)
        {
            return $"{Context.Configuration.RedisPrefix + domain}.";
        }
    }
}