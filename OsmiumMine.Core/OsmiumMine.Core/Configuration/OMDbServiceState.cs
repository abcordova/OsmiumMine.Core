using OsmiumMine.Core.Services.Security;
using System.Collections.Generic;

namespace OsmiumMine.Core.Configuration
{
    public class OMDbServiceState
    {
        public Dictionary<string, List<SecurityRule>> SecurityRuleTable { get; set; } = new Dictionary<string, List<SecurityRule>>();
    }
}