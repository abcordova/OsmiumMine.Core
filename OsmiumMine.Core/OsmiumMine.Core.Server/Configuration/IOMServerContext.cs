using OsmiumMine.Core.Server.Configuration.Access;
using OsmiumMine.Core.Services.Database;
using OsmiumSubstrate.Configuration;

namespace OsmiumMine.Core.Server.Configuration
{
    public interface IOMServerContext : ISubstrateServerContext<OMAccessKey, OMApiAccessScope>
    {
        OMServerParameters Parameters { get; set; }
        OsmiumMineContext OMContext { get; set; }
        KeyValueDatabaseService KeyValueDbService { get; set; }
        OMServerState ServerState { get; set; }
    }
}