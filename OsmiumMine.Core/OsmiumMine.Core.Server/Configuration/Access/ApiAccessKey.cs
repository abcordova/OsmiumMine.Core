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
        Default = 0,

        [EnumMember(Value = "admin")]
        Admin = 1,
    }

    public class ApiAccessKey
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("accessScope")]
        public ApiAccessScope AccessScope { get; set; } = ApiAccessScope.Default;

        [JsonProperty("rules")]
        public List<SecurityRule> SecurityRules { get; set; } = new List<SecurityRule>();

        [JsonProperty("realms")]
        public List<string> AllowedRealms { get; set; } = new List<string>();
    }
}