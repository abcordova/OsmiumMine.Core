using OsmiumMine.Core.Services.Security;
using System.Collections.Generic;
using System.Net;

namespace OsmiumMine.Core.Configuration
{
    public class OsmiumMineConfiguration
    {
        public int RedisPort { get; set; } = 6379;
        public string RedisAddress { get; set; } = IPAddress.Loopback.MapToIPv4().ToString();
        public string RedisPrefix { get; set; } = "osmiummine.";
        public Dictionary<string, SecurityRuleCollection> SecurityRuleTable { get; set; } = new Dictionary<string, SecurityRuleCollection>();

        public string RedisConnectionString => $"{RedisAddress}:{RedisPort}";
    }
}