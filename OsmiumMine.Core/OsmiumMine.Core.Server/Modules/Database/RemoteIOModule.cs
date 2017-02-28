using OsmiumMine.Core.Services.DynDatabase;
using OsmiumMine.Core.Services.DynDatabase.Access;
using OsmiumMine.Core.Services.Security;
using OsmiumMine.Core.Utilities;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace OsmiumMine.Core.Server.Modules.Database
{
    public class RemoteIOModule : NancyModule
    {
        public RemoteIOModule() : base("/io")
        {
            var pathRoutes = new[] { @"^(?:(?<dbid>[\w]+)\/(?<path>.*?)(?=\.json))" };
            foreach (var pathRoute in pathRoutes)
            {
                Put(pathRoute, HandlePutData);
                Patch(pathRoute, HandlePatchData);
                Post(pathRoute, HandlePostData);
                Delete(pathRoute, HandleDeleteData);
                Get(pathRoute, HandleGetData);
            }

            // Security setup
            Post("/addrule/{dbid}", HandleAddRuleRequest);

            // Do some initial setup
            Before += ctx =>
            {
                return null;
            };
            // Check requested response type argument
            // Refer to https://firebase.google.com/docs/database/rest/retrieve-data
            After += ctx =>
            {
                var queryData = ctx.Request.Query;
                var printArgument = (string)queryData.print;
                switch (printArgument)
                {
                    case "silent": // User does not want a response.
                        ctx.Response = null;
                        ctx.Response = HttpStatusCode.NoContent;
                        break;

                    default:
                        break;
                }
            };
        }

        private async Task<Response> HandleAddRuleRequest(dynamic args)
        {
            var dbid = (string)args.dbid;
            if (!OsmiumMine.CoreRegistry.Instance.Configuration.SecurityRuleTable.ContainsKey(dbid))
            {
                OsmiumMine.CoreRegistry.Instance.Configuration.SecurityRuleTable.Add(dbid, new SecurityRuleCollection());
            }
            OsmiumMine.CoreRegistry.Instance.Configuration.SecurityRuleTable[dbid].Add(SecurityRule.FromWildcard("/*", DatabaseAction.All));
            return HttpStatusCode.OK;
        }

        private async Task<Response> HandlePutData(dynamic args)
        {
            var dbRequest = (DynDatabaseRequest)DynDatabaseRequestProcessor.Process(args, DatabaseAction.Put);
            if (dbRequest.PermissionState == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            var path = dbRequest.Path;
            // Deserialize data bundle
            JObject dataBundle;
            try
            {
                dataBundle = JObject.Parse(Request.Body.AsString());
            }
            catch (JsonSerializationException)
            {
                return HttpStatusCode.BadRequest;
            }

            // Write data
            var placeResult = await DynamicDatabaseService.PlaceData(dataBundle, dbRequest.DatabaseId, dbRequest.Path, NodeDataOvewriteMode.Put);

            // Return data written
            return Response.FromJsonString(placeResult);
        }

        private async Task<Response> HandlePatchData(dynamic args)
        {
            var dbRequest = (DynDatabaseRequest)DynDatabaseRequestProcessor.Process(args, DatabaseAction.Update);
            if (dbRequest.PermissionState == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            // Deserialize data bundle
            JObject dataBundle;
            try
            {
                dataBundle = JObject.Parse(Request.Body.AsString());
            }
            catch (JsonSerializationException)
            {
                return HttpStatusCode.BadRequest;
            }

            // Write data
            var placeResult = await DynamicDatabaseService.PlaceData(dataBundle, dbRequest.DatabaseId, dbRequest.Path, NodeDataOvewriteMode.Update);

            // Return data written
            return Response.FromJsonString(placeResult);
        }

        private async Task<Response> HandlePostData(dynamic args)
        {
            var dbRequest = (DynDatabaseRequest)DynDatabaseRequestProcessor.Process(args, DatabaseAction.Push);
            if (dbRequest.PermissionState == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            // Deserialize data bundle
            JObject dataBundle;
            try
            {
                dataBundle = JObject.Parse(Request.Body.AsString());
            }
            catch (JsonSerializationException)
            {
                return HttpStatusCode.BadRequest;
            }

            // Write data
            var pushId = await DynamicDatabaseService.PlaceData(dataBundle, dbRequest.DatabaseId, dbRequest.Path, NodeDataOvewriteMode.Push);

            // Return data written
            return Response.AsJsonNet(new { name = pushId });
        }

        private async Task<Response> HandleDeleteData(dynamic args)
        {
            var dbRequest = (DynDatabaseRequest)DynDatabaseRequestProcessor.Process(args, DatabaseAction.Delete);
            if (dbRequest.PermissionState == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            await DynamicDatabaseService.DeleteData(dbRequest.DatabaseId, dbRequest.Path);

            return Response.FromJsonString(new JObject().ToString());
        }

        private async Task<Response> HandleGetData(dynamic args)
        {
            var dbRequest = (DynDatabaseRequest)DynDatabaseRequestProcessor.Process(args, DatabaseAction.Retreive);
            if (dbRequest.PermissionState == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            // Read query parameters
            var shallow = (string)Request.Query.shallow == "true";
            var dataBundle = await DynamicDatabaseService.GetData(dbRequest.DatabaseId, dbRequest.Path, shallow);

            if (dataBundle == null)
            {
                //return HttpStatusCode.NotFound;
                return Response.FromJsonString("{}");
            }

            return Response.FromJsonString(dataBundle.ToString());
        }
    }
}