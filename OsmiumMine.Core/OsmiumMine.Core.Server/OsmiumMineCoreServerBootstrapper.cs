using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using OsmiumMine.Core.Server.Configuration;

namespace OsmiumMine.Core.Server
{
    public class OsmiumMineCoreServerBootstrapper : DefaultNancyBootstrapper
    {
        public OMCoreServerConfiguration OMConfiguration { get; set; }

        public OsmiumMineCoreServerBootstrapper(OMCoreServerConfiguration configuration)
        {
            OMConfiguration = configuration;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            OMConfiguration.ConnectOsmiumMine();

            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IOMCoreServerConfiguration>(OMConfiguration);
        }
    }
}