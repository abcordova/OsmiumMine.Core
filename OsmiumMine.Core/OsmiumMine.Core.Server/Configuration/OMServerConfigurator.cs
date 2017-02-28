namespace OsmiumMine.Core.Server.Configuration
{
    public static class OMServerConfigurator
    {
        public static OMCoreServerConfiguration GetConfiguration(OMCoreServerParameters parameters)
        {
            // Load the configuration
            var config = new OMCoreServerConfiguration(parameters)
            {
                OMContext = new OsmiumMineContext()
                {
                    Configuration = parameters.OMConfig
                }
            };
            return config;
        }
    }
}