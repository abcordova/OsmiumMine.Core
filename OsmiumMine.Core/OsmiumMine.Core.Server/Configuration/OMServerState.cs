using LiteDB;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Server.Configuration.Access;
using System;
using System.Collections.Generic;

namespace OsmiumMine.Core.Server.Configuration
{
    /// <summary>
    /// Persisted state for the server
    /// </summary>
    public class OMServerState
    {
        public List<ApiAccessKey> ApiKeys { get; set; } = new List<ApiAccessKey>();

        // Proxy to context
        public OMServiceState DbServiceState { get; set; }

        [BsonIgnore]
        public LiteCollection<OMServerState> PersistenceMedium { get; set; }

        public Action Persist { get; set; }
    }
}