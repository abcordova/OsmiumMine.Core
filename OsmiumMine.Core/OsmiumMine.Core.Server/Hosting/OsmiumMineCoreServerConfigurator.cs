using OsmiumMine.Core.Configuration;

namespace OsmiumMine.Core.Server.Hosting
{
    public static class OsmiumMineCoreServerConfigurator
    {
        public static OsmiumMineConfiguration GetConfiguration()
        {
            var config = new OsmiumMineConfiguration();
            OsmiumMineCoreServerRegistry.ConnectOsmiumMine(config);
            return config;
        }
    }
}