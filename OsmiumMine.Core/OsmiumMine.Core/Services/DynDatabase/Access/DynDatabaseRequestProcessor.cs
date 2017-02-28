namespace OsmiumMine.Core.Services.DynDatabase.Access
{
    public class DynDatabaseRequestProcessor
    {
        public OsmiumMineContext Context { get; set; }

        public DynDatabaseRequestProcessor(OsmiumMineContext context)
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
                Path = path,
                DatabaseId = databaseId,
                Valid = databaseId != null,
                PermissionState = PermissionState.Denied
            };
            // TODO: Check permissions
            dbRequest.PermissionState = PermissionState.Denied;
            if (Context.Configuration.SecurityRuleTable.ContainsKey(databaseId))
            {
                // Rules are sorted in ascending priority order
                foreach (var rule in Context.Configuration.SecurityRuleTable[databaseId])
                {
                    if (rule.PathRegex.Match($"/{path}").Success)
                    {
                        if (rule.AllowedActions.HasFlag(dbAction))
                        {
                            dbRequest.PermissionState = PermissionState.Granted;
                            break;
                        }
                    }
                }
            }

            return dbRequest;
        }
    }
}