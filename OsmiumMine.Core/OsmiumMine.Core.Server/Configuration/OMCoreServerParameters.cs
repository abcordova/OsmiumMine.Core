using Newtonsoft.Json;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Server.Configuration.Access;

namespace OsmiumMine.Core.Server.Configuration
{
    /// <summary>
    /// Parameter model used to set up OMCoreServerConfiguration
    /// </summary>
    public class OMCoreServerParameters
    {
        [JsonProperty("omconfig")]
        public OsmiumMineConfiguration OsmiumMineConfiguration { get; set; } = new OsmiumMineConfiguration();

        [JsonProperty("apiKeys")]
        public ApiAccessKey[] ApiKeys { get; set; } = new ApiAccessKey[0];
    }
}