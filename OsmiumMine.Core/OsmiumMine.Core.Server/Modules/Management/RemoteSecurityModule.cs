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
        public IOMServerContext OMServerConfiguration { get; set; }

        public RemoteSecurityModule(IOMServerContext omConfiguration) : base("/rsec")
        {
            OMServerConfiguration = omConfiguration;

            var accessValidator = new ClientApiAccessValidator();
            this.RequiresAllClaims(accessValidator.GetAccessClaimListFromScopes(new[] {
                ApiAccessScope.Admin
            }), accessValidator.GetAccessClaim(ApiAccessScope.Admin));

            // Security setup
            Post("/rules/create/{dbid}", HandleCreateRuleRequestAsync);
            Delete("/rules/clear/{dbid}", HandleClearRulesRequestAsync);
            Delete("/rules/delete/{dbid}", HandleDeleteRuleRequestAsync);
            Get("/rules/list/{dbid}", HandleGetRuleListRequestAsync);
            Get("/rules/get/{dbid}", HandleGetRuleByIdRequestAsync);
        }

        private static (string, string) ParseRequestArgs(dynamic args, Request request, bool parsePathPattern = true)
        {
            var dbid = (string)args.dbid;
            var path = (string)request.Query.path;
            var wc = (string)request.Query.wc == "1"; // whether to parse as a wildcard
            if (string.IsNullOrWhiteSpace(dbid)) dbid = null;
            if (string.IsNullOrWhiteSpace(path)) path = parsePathPattern ? WildcardMatcher.ToRegex("/*") : "/";
            if (wc)
            {
                // convert wildcard to regex
                path = WildcardMatcher.ToRegex(path);
            }
            else
            {
                if (parsePathPattern)
                {
                    try
                    {
                        Regex.IsMatch("", path);
                    }
                    catch
                    {
                        // Path regex was invalid, parse as wildcard
                        path = WildcardMatcher.ToRegex(path);
                    }
                }
            }
            return (dbid, path);
        }

        private async Task<Response> HandleClearRulesRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args, Request, false);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                if (!OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                var dbRules = OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable[dbid];
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

        private async Task<Response> HandleDeleteRuleRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                // Path is not necessary for this operation
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args, Request, false);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                var ruleId = (string)Request.Query.id;
                if (string.IsNullOrWhiteSpace(ruleId)) return HttpStatusCode.BadRequest;
                if (!OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                var dbRules = OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable[dbid];
                // remove all rules that match the given path
                var removed = false;
                foreach (var dbRule in dbRules)
                {
                    if (dbRule.Id == ruleId)
                    {
                        dbRules.Remove(dbRule);
                        removed = true;
                        break;
                    }
                }
                return removed ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            });
        }

        private async Task<Response> HandleGetRuleByIdRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                // Path is not necessary for this operation
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args, Request, false);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                var ruleId = (string)Request.Query.id;
                if (string.IsNullOrWhiteSpace(ruleId)) return HttpStatusCode.BadRequest;
                if (!OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                var dbRules = OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable[dbid];
                // remove all rules that match the given path
                SecurityRule foundRule = null;
                foreach (var dbRule in dbRules)
                {
                    if (dbRule.Id == ruleId)
                    {
                        foundRule = dbRule;
                        break;
                    }
                }
                return foundRule != null ? Response.AsJsonNet(foundRule) : HttpStatusCode.NotFound;
            });
        }

        private async Task<Response> HandleGetRuleListRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args, Request, false);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                if (!OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                var matchingRules = OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable[dbid].Where(x => x.PathRegex.IsMatch(path));
                return Response.AsJsonNet(matchingRules);
            });
        }

        private async Task<Response> HandleCreateRuleRequestAsync(dynamic args)
        {
            return await Task.Run(() =>
            {
                (string dbid, string path) = ParseRequestArgs((DynamicDictionary)args, Request);
                if (string.IsNullOrWhiteSpace(dbid)) return HttpStatusCode.BadRequest;
                var allowRule = (string)Request.Query.allow == "1"; // whether the rule should set allow to true
                int priority = -1;
                int.TryParse((string)Request.Query.priority, out priority);
                DatabaseAction ruleFlags = 0;
                try
                {
                    var ruleTypes = ((string)Request.Query.type).Split('|').Select(x => (DatabaseAction)Enum.Parse(typeof(DatabaseAction), x));
                    foreach (var ruleType in ruleTypes)
                    {
                        ruleFlags |= ruleType;
                    }
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
                if (!OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.ContainsKey(dbid))
                {
                    OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
                }
                var rule = new SecurityRule(new Regex(path), ruleFlags, allowRule, priority);
                OMServerConfiguration.OMContext.ServiceState.SecurityRuleTable[dbid].Add(rule);
                return Response.AsJsonNet(rule);
            });
        }
    }
}