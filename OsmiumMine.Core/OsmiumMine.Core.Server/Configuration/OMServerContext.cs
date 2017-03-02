using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Server.Configuration
{
    public class OMServerContext : IOMServerContext
    {
        public OMServerParameters Parameters { get; set; }
        public OMServerState ServerState { get; set; }

        // Proxy to ServerState
        public OsmiumMineContext OMContext
        {
            get => ServerState.OMContext;
            set { ServerState.OMContext = value; }
        }

        public KeyValueDatabaseService KeyValueDbService { get; set; }

        public OMServerContext(OMServerParameters parameters)
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