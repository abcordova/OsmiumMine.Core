using Newtonsoft.Json;

namespace OsmiumMine.Core.Server.Configuration.Access
{
    /// <summary>
    /// An enumeration of access scope identifiers, used for granular permission grants on key access
    /// </summary>
    public enum ApiAccessScope
    {
        // General
        Read,
        Write,
        Admin,

        // Types of data
    }

    public class ApiAccessKey
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("scopes")]
        public ApiAccessScope[] AccessScopes { get; set; } = new[] { ApiAccessScope.Read };
    }
}