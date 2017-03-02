using OsmiumMine.Core.Configuration;

namespace OsmiumMine.Core
{
    public class OsmiumMineContext
    {
        public OMDatabaseConfiguration Configuration { get; set; }
        public OMDbServiceState ServiceState { get; set; }
    }
}