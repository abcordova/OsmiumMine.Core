using System;
using OsmiumMine.Core.Server.Configuration.Access;
using OsmiumMine.Core.Services.Database;
using OsmiumSubstrate.Configuration;

namespace OsmiumMine.Core.Server.Configuration
{
    public class OMServerContext : IOMServerContext
    {
        public OMServerParameters Parameters { get; set; }
        public OMServerState ServerState { get; set; }
        public OsmiumMineContext OMContext { get; set; }
        public KeyValueDatabaseService KeyValueDbService { get; set; }

        public ISubstrateServerState<OMAccessKey, OMApiAccessScope> SubstrateServerState => ServerState;

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