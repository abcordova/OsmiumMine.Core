using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Server
{
    public static class OsmiumMineCoreServerRegistry
    {
        public static OsmiumMineContext OMContext { get; set; }
        public static KeyValueDatabaseService KeyValueDbService { get; set; }

        public static void ConnectOsmiumMine(OsmiumMineConfiguration configuration)
        {
            OMContext = new OsmiumMineContext
            {
                Configuration = configuration
            };
            KeyValueDbService = new KeyValueDatabaseService(OMContext);
        }
    }
}