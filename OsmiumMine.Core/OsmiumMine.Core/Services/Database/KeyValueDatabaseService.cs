using OsmiumMine.Core.Services.Database.Redis;

namespace OsmiumMine.Core.Services.Database
{
    public class KeyValueDatabaseService
    {
        public OsmiumMineContext Context { get; set; }
        public IKeyValueDatabase Store { get; }

        public KeyValueDatabaseService(OsmiumMineContext context)
        {
            Context = context;
            Store = new RedisDatabase(Context.Configuration.RedisConnectionString);
        }

        /// <summary>
        /// Gets the data store prefix key
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