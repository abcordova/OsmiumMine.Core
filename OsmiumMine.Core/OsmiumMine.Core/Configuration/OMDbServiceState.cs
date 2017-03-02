using OsmiumMine.Core.Services.Security;
using System.Collections.Generic;

namespace OsmiumMine.Core.Configuration
{
    public class OMDbServiceState
    {
        public Dictionary<string, SecurityRuleCollection> SecurityRuleTable { get; set; } = new Dictionary<string, SecurityRuleCollection>();
    }
}