namespace OsmiumMine.Core.Server.Configuration
{
    public static class OMServerConfigurator
    {
        public static OMServerContext GetConfiguration(OMServerParameters parameters)
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
    }
}