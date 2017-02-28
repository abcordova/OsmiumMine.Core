using Nancy;

namespace OsmiumMine.Core.Server.Modules
{
    public class OsmiumMineServerModule : NancyModule
    {
        public OsmiumMineServerModule(string path) : base($"/om/{path}")
        {
        }
    }
}