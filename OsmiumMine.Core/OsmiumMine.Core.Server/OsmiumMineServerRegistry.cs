using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Server
{
    public static class OsmiumMineServerRegistry
    {
        public static OsmiumMineContext OMContext { get; set; }
        public static KeyValueDatabaseService KeyValueDbService { get; set; }

        public static void ConnectOsmiumMine()
        {
            OMContext = new OsmiumMineContext
            {
                Configuration = new OsmiumMineConfiguration()
                {
                }
            };
            KeyValueDbService = new KeyValueDatabaseService(OMContext);
        }
    }
}