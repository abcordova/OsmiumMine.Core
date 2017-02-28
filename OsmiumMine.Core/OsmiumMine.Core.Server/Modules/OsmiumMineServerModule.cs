using Nancy;

namespace OsmiumMine.Core.Server.Modules
{
    public abstract class OsmiumMineServerModule : NancyModule
    {
        internal OsmiumMineServerModule(string path) : base($"/om/{path}")
        {
        }
    }
}