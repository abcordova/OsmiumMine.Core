using Newtonsoft.Json;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Server.Configuration.Access;

namespace OsmiumMine.Core.Server.Configuration
{
    /// <summary>
    /// Parameter model used to set up OMCoreServerConfiguration.
    /// These parameters are used for initializing the server and connecting to the database.
    /// </summary>
    public class OMServerParameters
    {
        [JsonProperty("omconfig")]
        public OMDatabaseConfiguration OMConfig { get; set; } = new OMDatabaseConfiguration();

        /// <summary>
        /// Master API keys. These will also be stored in the state but will not be duplicated.
        /// </summary>
        [JsonProperty("apikeys")]
        public ApiAccessKey[] ApiKeys { get; set; } = new ApiAccessKey[0];
    }
}