using System.Linq;
using LiteDB;
using OsmiumMine.Core.Configuration;

namespace OsmiumMine.Core.Server.Configuration
{
    public static class OMServerConfigurator
    {
        public const string StateStorageKey = "state";

        public static bool DatabaseMappersRegistered { get; private set; }

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

        public static void LoadState(OMServerContext serverContext, string stateStorageFile)
        {
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
            // Apply key reset
            if (serverContext.Parameters.KeyReset)
                savedState.ApiKeys.Clear();
            // Merge the API keys
            foreach (var paramKey in serverContext.Parameters.ApiKeys)
            {
                // look for a matching key
                var existingKey = savedState.ApiKeys.FirstOrDefault(x => x.Key == paramKey.Key);
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