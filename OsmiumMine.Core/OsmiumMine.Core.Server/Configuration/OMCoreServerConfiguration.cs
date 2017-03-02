using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Server.Configuration
{
    public class OMCoreServerConfiguration : IOMCoreServerConfiguration
    {
        public OMServerParameters Parameters { get; set; }
        public OsmiumMineContext OMContext { get; set; }
        public KeyValueDatabaseService KeyValueDbService { get; set; }

        public OMCoreServerConfiguration(OMServerParameters parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Load context and configuration and instantiate services
        /// </summary>
        public void ConnectOsmiumMine()
        {
            KeyValueDbService = new KeyValueDatabaseService(OMContext);
        }
    }
}