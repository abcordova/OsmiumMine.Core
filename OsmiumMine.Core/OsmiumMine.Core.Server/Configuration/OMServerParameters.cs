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
        /// If set to true, the keys will be reset upon a server start.
        /// </summary>
        public bool KeyReset { get; set; } = false;

        /// <summary>
        /// Master API keys. These will also be stored in the state but will not be duplicated.
        /// </summary>
        [JsonProperty("apikeys")]
        public OMAccessKey[] ApiKeys { get; set; } = new OMAccessKey[0];

        [JsonProperty("corsOrigins")]
        public string[] CorsOrigins { get; set; } = new string[0];
    }
}