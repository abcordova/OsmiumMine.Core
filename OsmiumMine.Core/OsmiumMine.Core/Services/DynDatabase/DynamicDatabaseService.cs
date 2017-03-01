using IridiumIon.JsonFlat3;
using Newtonsoft.Json.Linq;
using OsmiumMine.Core.Services.Database;
using OsmiumMine.Core.Services.DynDatabase.Access;
using OsmiumMine.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsmiumMine.Core.Services.DynDatabase
{
    public class DynamicDatabaseService
    {
        public enum NodeDataOvewriteMode
        {
            Put,
            Update,
            Push
        }

        public KeyValueDatabaseService DbService { get; set; }

        public DynamicDatabaseService(KeyValueDatabaseService keyValueDatabase)
        {
            DbService = keyValueDatabase;
        }

        public async Task<string> PlaceData(JObject dataBundleRoot, DynDatabaseRequest request, NodeDataOvewriteMode overwriteMode, bool response = true)
        {
            var realmDomain = DbService.GetDomainPath(request.DatabaseId);
            string result = null; // Optionally used to return a result
            var dataPath = new FlatJsonPath(request.Path);
            await Task.Run(() =>
            {
                // Parse server values
                if (dataBundleRoot.Children().Count() == 1
                    && dataBundleRoot.First is JProperty
                    && (dataBundleRoot.First as JProperty).Name == ServerValueProvider.ServerValueKeyName)
                {
                    var serverValueToken = ((JProperty)dataBundleRoot.First).Value;
                    var serverValueName = (serverValueToken is JValue) ? ((serverValueToken as JValue).Value as string) : null;
                    var resultValue = ServerValueProvider.GetValue(serverValueName);
                    // Now adjust path
                    // Get the last segment, then remove it
                    var lastSegment = dataPath.Segments.Last();
                    dataPath.Segments.RemoveAt(dataPath.Segments.Count - 1);
                    dataBundleRoot = new JObject(new JProperty(lastSegment, resultValue));
                }

                // Put in the new data
                switch (overwriteMode)
                {
                    case NodeDataOvewriteMode.Update:
                        {
                            // Flatten input bundle
                            var flattenedBundle = new FlatJsonObject(dataBundleRoot, dataPath.TokenPrefix);
                            // Merge input bundle
                            foreach (var kvp in flattenedBundle.Dictionary)
                            {
                                DbService.Store.Set(realmDomain, kvp.Key, kvp.Value);
                            }
                        }

                        break;

                    case NodeDataOvewriteMode.Put:
                        {
                            // Flatten input bundle
                            var dataTokenPrefix = dataPath.TokenPrefix;
                            // Get existing data
                            var flattenedBundle = new FlatJsonObject(dataBundleRoot, dataTokenPrefix);
                            var existingData = DbService.Store.GetKeys(realmDomain).Where(x => x.StartsWith(dataTokenPrefix, StringComparison.Ordinal));
                            // Remove existing data
                            foreach (var key in existingData)
                            {
                                DbService.Store.Delete(realmDomain, key);
                            }
                            // Add input bundle
                            foreach (var kvp in flattenedBundle.Dictionary)
                            {
                                DbService.Store.Set(realmDomain, kvp.Key, kvp.Value);
                            }
                        }
                        break;

                    case NodeDataOvewriteMode.Push:
                        {
                            // Use the Firebase Push ID algorithm
                            var pushId = PushIdGenerator.GeneratePushId();
                            // Create flattened bundle with pushId added to prefix
                            dataPath.Segments.Add(pushId);
                            // Flatten input bundle
                            var flattenedBundle = new FlatJsonObject(dataBundleRoot, dataPath.TokenPrefix);
                            // Merge input bundle
                            foreach (var kvp in flattenedBundle.Dictionary)
                            {
                                DbService.Store.Set(realmDomain, kvp.Key, kvp.Value);
                            }
                        }
                        break;
                }

                var dataBundleJson = dataBundleRoot.ToString();

                // Data was written
                if (response) result = dataBundleJson;
            });
            return result;
        }

        public async Task DeleteData(DynDatabaseRequest request)
        {
            var domain = DbService.GetDomainPath(request.DatabaseId);
            var dataPath = new FlatJsonPath(request.Path);
            await Task.Run(() =>
            {
                // Get existing data
                var existingData = DbService.Store.GetKeys(domain).Where(x => x.StartsWith(dataPath.TokenPrefix, StringComparison.Ordinal));
                // Remove existing data
                foreach (var key in existingData)
                {
                    DbService.Store.Delete(domain, key);
                }
            });
        }

        public async Task<JToken> GetData(DynDatabaseRequest request, bool shallow = false)
        {
            var domain = DbService.GetDomainPath(request.DatabaseId);
            var dataPath = new FlatJsonPath(request.Path);
            return await Task.Run(() =>
            {
                if (!DbService.Store.Exists(domain)) return null; // 404 - There's no data container
                // Unflatten and retrieve JSON object
                var flatJsonDict = new Dictionary<string, string>();
                // Use an optimization to only fetch requested keys
                foreach (var key in DbService.Store.GetKeysEnumerable(domain).Where(x =>
                    x.StartsWith(dataPath.TokenPrefix, StringComparison.Ordinal)
                    || x.StartsWith(dataPath.ArrayPrefix, StringComparison.Ordinal)))
                {
                    var val = DbService.Store.Get(domain, key);
                    flatJsonDict.Add(key, val);
                }
                var unflattenedJObj = new FlatJsonObject(flatJsonDict).Unflatten();
                if (shallow)
                {
                    var resultObject = unflattenedJObj.SelectToken(dataPath.TokenPath);
                    if (resultObject is JArray)
                    {
                        return new JArray(resultObject.Children().Select(c => new JValue(c ?? false)));
                    }
                    else if (resultObject is JObject)
                    {
                        var filteredProps = (resultObject as JObject).Properties().Select(p => new JProperty(p.Name,
                            (p.Children().FirstOrDefault() is JObject ? true : p.Value) ?? false));
                        return new JObject(filteredProps);
                    }
                    return null;
                }
                else
                {
                    return unflattenedJObj.SelectToken(dataPath.TokenPath);
                }
            });
        }
    }
}