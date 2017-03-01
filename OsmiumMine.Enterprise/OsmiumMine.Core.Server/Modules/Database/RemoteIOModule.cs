using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OsmiumMine.Core.Server.Configuration;
using OsmiumMine.Core.Server.Utilities;
using OsmiumMine.Core.Services.DynDatabase;
using OsmiumMine.Core.Services.DynDatabase.Access;
using System.IO;
using System.Threading.Tasks;
using static OsmiumMine.Core.Services.DynDatabase.Access.DynDatabaseRequest;
using static OsmiumMine.Core.Services.DynDatabase.DynamicDatabaseService;

namespace OsmiumMine.Core.Server.Modules.Database
{
    public class RemoteIOModule : OsmiumMineServerModule
    {
        public IOMCoreServerConfiguration OMServerConfiguration { get; set; }

        public RemoteIOModule(IOMCoreServerConfiguration omConfiguration) : base("/io")
        {
            OMServerConfiguration = omConfiguration;

            var pathRoutes = new[] { @"^(?:(?<dbid>[\w]+)\/(?<path>.*?)(?=\.json))" };
            foreach (var pathRoute in pathRoutes)
            {
                Put(pathRoute, HandlePutDataAsync);
                Patch(pathRoute, HandlePatchDataAsync);
                Post(pathRoute, HandlePostDataAsync);
                Delete(pathRoute, HandleDeleteDataAsync);
                Get(pathRoute, HandleGetDataAsync);
            }

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

        private async Task<Response> HandlePutDataAsync(dynamic args)
        {
            var requestProcessor = new RequestProcessor(OMServerConfiguration.OMContext);
            var dbRequest = (DynDatabaseRequest)requestProcessor.Process(args, DatabaseAction.Put);
            if (dbRequest.State == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            var path = dbRequest.Path;
            // Deserialize data bundle
            JObject dataBundle;
            try
            {
                using (var sr = new StreamReader(Request.Body))
                {
                    var rawJsonData = await sr.ReadToEndAsync();
                    dataBundle = JObject.Parse(rawJsonData);
                }
            }
            catch (JsonSerializationException)
            {
                return HttpStatusCode.BadRequest;
            }

            // Write data
            var dynDbService = new DynamicDatabaseService(OMServerConfiguration.KeyValueDbService);
            var placeResult = await dynDbService.PlaceData(dataBundle, dbRequest.DatabaseId, dbRequest.Path, NodeDataOvewriteMode.Put);

            // Return data written
            return Response.FromJsonString(placeResult);
        }

        private async Task<Response> HandlePatchDataAsync(dynamic args)
        {
            var requestProcessor = new RequestProcessor(OMServerConfiguration.OMContext);
            var dbRequest = (DynDatabaseRequest)requestProcessor.Process(args, DatabaseAction.Update);
            if (dbRequest.State == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            // Deserialize data bundle
            JObject dataBundle;
            try
            {
                using (var sr = new StreamReader(Request.Body))
                {
                    var rawJsonData = await sr.ReadToEndAsync();
                    dataBundle = JObject.Parse(rawJsonData);
                }
            }
            catch (JsonSerializationException)
            {
                return HttpStatusCode.BadRequest;
            }

            // Write data
            var dynDbService = new DynamicDatabaseService(OMServerConfiguration.KeyValueDbService);
            var placeResult = await dynDbService.PlaceData(dataBundle, dbRequest.DatabaseId, dbRequest.Path, NodeDataOvewriteMode.Update);

            // Return data written
            return Response.FromJsonString(placeResult);
        }

        private async Task<Response> HandlePostDataAsync(dynamic args)
        {
            var requestProcessor = new RequestProcessor(OMServerConfiguration.OMContext);
            var dbRequest = (DynDatabaseRequest)requestProcessor.Process(args, DatabaseAction.Push);
            if (dbRequest.State == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            // Deserialize data bundle
            JObject dataBundle;
            try
            {
                using (var sr = new StreamReader(Request.Body))
                {
                    var rawJsonData = await sr.ReadToEndAsync();
                    dataBundle = JObject.Parse(rawJsonData);
                }
            }
            catch (JsonSerializationException)
            {
                return HttpStatusCode.BadRequest;
            }

            // Write data
            var dynDbService = new DynamicDatabaseService(OMServerConfiguration.KeyValueDbService);
            var pushId = await dynDbService.PlaceData(dataBundle, dbRequest.DatabaseId, dbRequest.Path, NodeDataOvewriteMode.Push);

            // Return data written
            return Response.AsJsonNet(new { name = pushId });
        }

        private async Task<Response> HandleDeleteDataAsync(dynamic args)
        {
            var requestProcessor = new RequestProcessor(OMServerConfiguration.OMContext);
            var dbRequest = (DynDatabaseRequest)requestProcessor.Process(args, DatabaseAction.Delete);
            if (dbRequest.State == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            var dynDbService = new DynamicDatabaseService(OMServerConfiguration.KeyValueDbService);
            await dynDbService.DeleteData(dbRequest.DatabaseId, dbRequest.Path);

            return Response.FromJsonString(new JObject().ToString());
        }

        private async Task<Response> HandleGetDataAsync(dynamic args)
        {
            var requestProcessor = new RequestProcessor(OMServerConfiguration.OMContext);
            var dbRequest = (DynDatabaseRequest)requestProcessor.Process(args, DatabaseAction.Retrieve);
            if (dbRequest.State == PermissionState.Denied) return HttpStatusCode.Unauthorized;
            if (!dbRequest.Valid) return HttpStatusCode.BadRequest;
            // Read query parameters
            var shallow = (string)Request.Query.shallow == "1";
            var dynDbService = new DynamicDatabaseService(OMServerConfiguration.KeyValueDbService);
            var dataBundle = await dynDbService.GetData(dbRequest.DatabaseId, dbRequest.Path, shallow);

            if (dataBundle == null)
            {
                //return HttpStatusCode.NotFound;
                return Response.FromJsonString("{}");
            }

            return Response.FromJsonString(dataBundle.ToString());
        }
    }
}