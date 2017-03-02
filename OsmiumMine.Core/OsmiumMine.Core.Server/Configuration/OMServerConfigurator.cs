using LiteDB;
using OsmiumMine.Core.Configuration;
using System.Linq;

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
            // Merge the API keys
            var existingApiKeys = savedState.ApiKeys.Select(x => x.Key);
            foreach (var paramApiKey in serverContext.Parameters.ApiKeys)
            {
                if (!existingApiKeys.Contains(paramApiKey.Key))
                {
                    savedState.ApiKeys.Add(paramApiKey);
                }
            }
            // Save the state
            savedState.PersistenceMedium.Upsert(savedState);
            // Store the database state in the context
            serverContext.OMContext.ServiceState = savedState.DbServiceState;
        }
    }
}