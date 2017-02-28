using Nancy;
using OsmiumMine.Core.Configuration;

namespace OsmiumMine.Core.Server
{
    public class OsmiumMineCoreServerBootstrapper : DefaultNancyBootstrapper
    {
        public OsmiumMineConfiguration Configuration { get; set; }

        public OsmiumMineCoreServerBootstrapper(OsmiumMineConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}