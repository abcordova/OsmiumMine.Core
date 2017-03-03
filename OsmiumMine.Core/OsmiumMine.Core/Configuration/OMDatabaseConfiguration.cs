using System.Net;

namespace OsmiumMine.Core.Configuration
{
    /// <summary>
    /// Configuration for interacting with the database
    /// </summary>
    public class OMDatabaseConfiguration
    {
        public int RedisPort { get; set; } = 6379;
        public string RedisAddress { get; set; } = IPAddress.Loopback.MapToIPv4().ToString();
        public string RedisPrefix { get; set; } = "osmiummine.";
        public string RedisConnectionString => $"{RedisAddress}:{RedisPort}";
    }
}