using Newtonsoft.Json;
using OsmiumMine.Core.Services.Security;
using OsmiumSubstrate.Configuration.Access;
using System.Collections.Generic;

namespace OsmiumMine.Core.Server.Configuration.Access
{
    /// <summary>
    /// An enumeration of access scope identifiers, used for granular permission grants on key access
    /// </summary>
    public enum OMApiAccessScope
    {
        Default,
        Admin,
    }

    public class OMAccessKey : AccessKey<OMApiAccessScope>
    {
        [JsonProperty("rules")]
        public List<SecurityRule> SecurityRules { get; set; } = new List<SecurityRule>();

        [JsonProperty("realms")]
        public List<string> AllowedRealms { get; set; } = new List<string>();

        [JsonProperty("scopes")]
        public override OMApiAccessScope[] AccessScopes { get; set; } = new[] { OMApiAccessScope.Default };
    }
}