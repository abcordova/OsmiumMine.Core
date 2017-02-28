namespace OsmiumMine.Core.Server.Configuration
{
    public static class OMServerConfigurator
    {
        public static OMCoreServerConfiguration GetConfiguration()
        {
            // Load the configuration
            return new OMCoreServerConfiguration();
        }
    }
}