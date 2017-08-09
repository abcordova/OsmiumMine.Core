using System.Collections.Generic;
using OsmiumMine.Core.Services.Security;

namespace OsmiumMine.Core.Configuration
{
    public class OMDbServiceState
    {
        public Dictionary<string, List<SecurityRule>> SecurityRuleTable { get; set; } =
            new Dictionary<string, List<SecurityRule>>();
    }
}