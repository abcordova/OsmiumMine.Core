using System;
using System.Collections.Generic;
using LiteDB;
using OsmiumMine.Core.Configuration;
using OsmiumMine.Core.Server.Configuration.Access;
using OsmiumSubstrate.Configuration;

namespace OsmiumMine.Core.Server.Configuration
{
    /// <summary>
    ///     Persisted state for the server
    /// </summary>
    public class OMServerState : ISubstrateServerState<OMAccessKey, OMApiAccessScope>
    {
        public List<OMAccessKey> ApiKeys { get; set; } = new List<OMAccessKey>();


        // Proxy to context
        public OMDbServiceState DbServiceState { get; set; }

        /// <summary>
        ///     BsonDocument ID
        /// </summary>
        [BsonId]
        public ObjectId DatabaseId { get; set; }

        [BsonIgnore]
        public LiteCollection<OMServerState> PersistenceMedium { get; set; }

        [BsonIgnore]
        public Action Persist { get; set; }

        [BsonIgnore]
        IEnumerable<OMAccessKey> ISubstrateServerState<OMAccessKey, OMApiAccessScope>.ApiKeys => ApiKeys;
    }
}