using OsmiumMine.Core.Server.Configuration.Access;
using System.Collections.Generic;

namespace OsmiumMine.Core.Server.Configuration
{
    /// <summary>
    /// Persisted state for the server
    /// </summary>
    public class OMServerState
    {
        public OsmiumMineContext OMContext { get; set; }
        public List<ApiAccessKey> ApiKeys { get; set; } = new List<ApiAccessKey>();
    }
}