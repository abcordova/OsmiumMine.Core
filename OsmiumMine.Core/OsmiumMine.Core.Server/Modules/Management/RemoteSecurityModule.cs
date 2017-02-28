using Nancy;
using OsmiumMine.Core.Server.Configuration;
using OsmiumMine.Core.Services.DynDatabase.Access;
using OsmiumMine.Core.Services.Security;
using System.Threading.Tasks;

namespace OsmiumMine.Core.Server.Modules.Management
{
    public class RemoteSecurityModule : OsmiumMineServerModule
    {
        public IOMCoreServerConfiguration OMServerConfiguration { get; set; }

        public RemoteSecurityModule(IOMCoreServerConfiguration omConfiguration) : base("/rsec")
        {
            OMServerConfiguration = omConfiguration;

            // Security setup
            Post("/addrule/{dbid}", HandleAddRuleRequestAsync);
        }

        private async Task<Response> HandleAddRuleRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                var dbid = (string)args.dbid;
                if (!OMServerConfiguration.OMContext.Configuration.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.Configuration.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                OMServerConfiguration.OMContext.Configuration.SecurityRuleTable[dbid].Add(SecurityRule.FromWildcard("/*", DatabaseAction.All));
                return HttpStatusCode.OK;
            });
        }
    }
}