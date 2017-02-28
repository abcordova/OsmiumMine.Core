namespace OsmiumMine.Core.Server.Configuration
{
    public static class OMServerConfigurator
    {
        public static OMCoreServerConfiguration GetConfiguration(OMCoreServerParameters serverParams)
        {
            // Load the configuration
            var config = new OMCoreServerConfiguration()
            {
                OMContext = new OsmiumMineContext()
                {
                    Configuration = serverParams.OsmiumMineConfiguration
                }
            };
            return config;
        }
    }
}