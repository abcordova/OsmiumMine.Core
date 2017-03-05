using OsmiumMine.Core.Server.Configuration;

namespace OsmiumMine.Core.Server.Modules.Metadata
{
    public class RemoteIOModule : OsmiumMineServerModule
    {
        public IOMServerContext ServerContext { get; set; }

        public RemoteIOModule(IOMServerContext serverContext) : base("/a")
        {
            ServerContext = serverContext;

            Get("/info", _ => "OsmiumMine.Core.Server Standalone Edition");
        }
    }
}