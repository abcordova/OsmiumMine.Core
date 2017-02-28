using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Server.Configuration
{
    public interface IOMCoreServerConfiguration
    {
        OsmiumMineContext OMContext { get; set; }
        KeyValueDatabaseService KeyValueDbService { get; set; }
    }
}