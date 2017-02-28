using Newtonsoft.Json;
using OsmiumMine.Core.Configuration;

namespace OsmiumMine.Core.Server.Configuration
{
    public class OMCoreServerParameters
    {
        [JsonProperty("omconfig")]
        public OsmiumMineConfiguration OsmiumMineConfiguration { get; set; }
    }
}