using Newtonsoft.Json;
using OsmiumMine.Core.Services.Security;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OsmiumMine.Core.Server.Configuration.Access
{
    /// <summary>
    /// An enumeration of access scope identifiers, used for granular permission grants on key access
    /// </summary>
    public enum ApiAccessScope
    {
        // General
        [EnumMember(Value = "default")]
        Read = 0,

        [EnumMember(Value = "admin")]
        Admin = 1,
    }

    public class ApiAccessKey
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("accessScopes")]
        public ApiAccessScope[] AccessScopes { get; set; } = new ApiAccessScope[0];

        [JsonProperty("rules")]
        public List<SecurityRule> SecurityRules { get; set; } = new List<SecurityRule>();

        [JsonProperty("realms")]
        public List<string> AllowedRealms { get; set; } = new List<string>();
    }
}