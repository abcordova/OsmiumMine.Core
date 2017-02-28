using Nancy;
using OsmiumMine.Core.Server.Configuration;
using OsmiumMine.Core.Server.Configuration.Access;
using OsmiumMine.Core.Server.Services.Authentication;
using OsmiumMine.Core.Server.Services.Authentication.Security;
using OsmiumMine.Core.Server.Utilities;
using OsmiumMine.Core.Services.DynDatabase.Access;
using OsmiumMine.Core.Services.Security;
using OsmiumMine.Core.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OsmiumMine.Core.Server.Modules.Management
{
    public class RemoteSecurityModule : OsmiumMineServerModule
    {
        public IOMCoreServerConfiguration OMServerConfiguration { get; set; }

        public RemoteSecurityModule(IOMCoreServerConfiguration omConfiguration) : base("/rsec")
        {
            OMServerConfiguration = omConfiguration;

            var accessValidator = new ClientApiAccessValidator();
            this.RequiresAllClaims(accessValidator.GetAccessClaimListFromScopes(new[] {
                ApiAccessScope.Admin
            }), accessValidator.GetAccessClaim(ApiAccessScope.Admin));

            // Security setup
            Post("/rules/create/{dbid}", HandleCreateRuleRequestAsync);
            Delete("/rules/clear/{dbid}", HandleClearRulesRequestAsync);
        }

        private static (string, string) ParseRequestArgs(dynamic args)
        {
            var dbid = (string)args.dbid;
            var path = (string)args.path;
            if (string.IsNullOrWhiteSpace(dbid)) dbid = null;
            if (string.IsNullOrWhiteSpace(path)) path = WildcardMatcher.ToRegex("/*");
            try
            {
                Regex.IsMatch("", path);
            }
            catch
            {
                // Path regex was invalid, parse as wildcard
                path = WildcardMatcher.ToRegex(path);
            }
            return (dbid, path);
        }

        private async Task<Response> HandleClearRulesRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                var dbRules = OMServerConfiguration.OMContext.Configuration.SecurityRuleTable[dbid];
                // remove all rules that match the given path
                foreach (var dbRule in dbRules)
                {
                    if (dbRule.PathRegex.IsMatch(path))
                    {
                        dbRules.Remove(dbRule);
                    }
                }
                return HttpStatusCode.OK;
            });
        }

        private async Task<Response> GetRuleListRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                var matchingRules = OMServerConfiguration.OMContext.Configuration.SecurityRuleTable[dbid].Where(x => x.PathRegex.IsMatch(path));
                return Response.AsJsonNet(matchingRules);
            });
        }

        private async Task<Response> HandleCreateRuleRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                DatabaseAction ruleFlags = 0;
                try
                {
                    var ruleTypes = ((string)args.type).Split('|').Select(x => (DatabaseAction)Enum.Parse(typeof(DatabaseAction), x));
                    foreach (var ruleType in ruleTypes)
                    {
                        ruleFlags |= ruleType;
                    }
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
                if (!OMServerConfiguration.OMContext.Configuration.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.Configuration.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                OMServerConfiguration.OMContext.Configuration.SecurityRuleTable[dbid].Add(SecurityRule.FromRegex(path, ruleFlags));
                return HttpStatusCode.OK;
            });
        }
    }
}