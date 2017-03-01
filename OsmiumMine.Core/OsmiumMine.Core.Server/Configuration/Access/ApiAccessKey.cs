using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OsmiumMine.Core.Server.Configuration.Access
{
    /// <summary>
    /// An enumeration of access scope identifiers, used for granular permission grants on key access
    /// </summary>
    public enum ApiAccessScope
    {
        // General
        [EnumMember(Value = "read")]
        Read,

        [EnumMember(Value = "write")]
        Write,

        [EnumMember(Value = "admin")]
        Admin,

        // Types of data
    }

    public class ApiAccessKey
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("accessScopes")]
        public ApiAccessScope[] AccessScopes { get; set; } = new[] { ApiAccessScope.Read };
    }
}