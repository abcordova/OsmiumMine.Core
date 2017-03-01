using System;
using static OsmiumMine.Core.Services.DynDatabase.Access.DynDatabaseRequest;

namespace OsmiumMine.Core.Services.DynDatabase.Access
{
    public class RequestProcessor
    {
        public OsmiumMineContext Context { get; set; }

        public RequestProcessor(OsmiumMineContext context)
        {
            Context = context;
        }

        public DynDatabaseRequest Process(dynamic args, DatabaseAction dbAction)
        {
            // Read basic parameters
            var path = (string)args.path ?? ""; // null paths aren't valid
            var databaseId = (string)args.dbid; // database ID is required!
            var authItem = (string)args.auth;
            // Create request shell
            var dbRequest = new DynDatabaseRequest
            {
                AuthToken = authItem,
                Path = path,
                DatabaseId = databaseId,
                Valid = databaseId != null,
                State = PermissionState.Denied,
                RequestedAction = dbAction
            };

            return ValidateAccess(dbRequest);
        }

        public DynDatabaseRequest ValidateAccess(DynDatabaseRequest dbRequest)
        {
            // Check permissions
            dbRequest.State = PermissionState.Denied;
            if (Context.Configuration.SecurityRuleTable.ContainsKey(dbRequest.DatabaseId))
            {
                // Rules are sorted in ascending priority order
                foreach (var rule in Context.Configuration.SecurityRuleTable[dbRequest.DatabaseId])
                {
                    if (rule.PathRegex.Match($"/{dbRequest.Path}").Success)
                    {
                        if (rule.Actions.HasFlag(dbRequest.RequestedAction))
                        {
                            dbRequest.State = rule.Allow ? PermissionState.Granted : PermissionState.Denied;
                            break;
                        }
                    }
                }
            }

            // If denied by rules, check auth token
            if (dbRequest.State == PermissionState.Denied)
            {
                if (AuthTokenValidator != null)
                {
                    if (AuthTokenValidator(dbRequest))
                    {
                        dbRequest.State = PermissionState.Granted;
                    }
                }
            }

            return dbRequest;
        }

        public Func<DynDatabaseRequest, bool> AuthTokenValidator { get; set; }
    }
}