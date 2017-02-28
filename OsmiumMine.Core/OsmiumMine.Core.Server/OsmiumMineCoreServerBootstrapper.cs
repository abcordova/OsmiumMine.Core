using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using OsmiumMine.Core.Server.Configuration;

namespace OsmiumMine.Core.Server
{
    public class OsmiumMineCoreServerBootstrapper : DefaultNancyBootstrapper
    {
        public OMCoreServerConfiguration Configuration { get; set; }

        public OsmiumMineCoreServerBootstrapper(OMCoreServerConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            Configuration.ConnectOsmiumMine();

            base.ApplicationStartup(container, pipelines);
        }
    }
}