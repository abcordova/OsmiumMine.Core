using OsmiumMine.Core.Services.Database.Redis;

namespace OsmiumMine.Core.Services.Database
{
    public class KeyValueDatabaseService
    {
        public static IKeyValueDatabase Store { get; } = new RedisDatabase();

        /// <summary>
        /// Gets the data store prefix key
        /// </summary>
        /// <param name="subDatabaseId"></param>
        /// <returns></returns>
        public static string GetDSPrefixKey(string subDatabaseId, bool includeSuffix = true)
        {
            return $"{OsmiumMine.CoreRegistry.Instance.Configuration.RedisPrefix + subDatabaseId}" + (includeSuffix ? "." : "");
        }

        public static string GetDomainPath(string domain)
        {
            return $"{OsmiumMine.CoreRegistry.Instance.Configuration.RedisPrefix + domain}";
        }

        public static string GetDomainSuffix(string domain)
        {
            return $"{OsmiumMine.CoreRegistry.Instance.Configuration.RedisPrefix + domain}.";
        }
    }
}