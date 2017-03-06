using LiteDB;
using OsmiumMine.Core.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace OsmiumMine.Core.Server.Configuration
{
    public static class OMServerConfigurator
    {
        public static OMServerContext CreateContext(OMServerParameters parameters)
        {
            // Load the configuration
            var config = new OMServerContext(parameters)
            {
                OMContext = new OsmiumMineContext
                {
                    Configuration = parameters.OMConfig
                }
            };
            return config;
        }

        public const string StateStorageKey = "state";

        public static bool DatabaseMappersRegistered { get; private set; }

        public static void RegisterDatabaseMappers()
        {
            // Register a Regex BSONMapper
            BsonMapper.Global.RegisterType<Regex>(
                serialize: r => r.ToString(),
                deserialize: d => new Regex(d.AsString)
            );
            DatabaseMappersRegistered = true;
        }

        public static void LoadState(OMServerContext serverContext, string stateStorageFile)
        {
            // Register mappers
            if (!DatabaseMappersRegistered) RegisterDatabaseMappers();
            // Load the Server State into the context. This object also includes the OsmiumMine Core state
            var database = new LiteDatabase(stateStorageFile);
            var stateStorage = database.GetCollection<OMServerState>(StateStorageKey);
            var savedState = stateStorage.FindAll().FirstOrDefault();
            if (savedState == null)
            {
                // Create and save new state
                savedState = new OMServerState
                {
                    DbServiceState = new OMDbServiceState()
                };
                stateStorage.Upsert(savedState);
            }
            // Update context
            savedState.PersistenceMedium = stateStorage;
            savedState.Persist = () =>
            {
                // Update in database
                stateStorage.Upsert(savedState);
            };
            // Merge the API keys
            var existingApiKeys = savedState.ApiKeys.Select(x => x.Key);
            foreach (var paramKey in serverContext.Parameters.ApiKeys)
            {
                var existingKey = serverContext.Parameters.ApiKeys.FirstOrDefault(x => x.Key == paramKey.Key);
                if (existingKey == null)
                {
                    savedState.ApiKeys.Add(paramKey);
                }
                else
                {
                    // Remove the old key and add the new key
                    savedState.ApiKeys.Remove(existingKey);
                    savedState.ApiKeys.Add(paramKey);
                }
            }
            // Save the state
            //savedState.PersistenceMedium.Upsert(savedState);
            savedState.Persist();
            // Update references
            serverContext.ServerState = savedState;
            // Store the database state in the context
            serverContext.OMContext.DbServiceState = savedState.DbServiceState;
        }
    }
}