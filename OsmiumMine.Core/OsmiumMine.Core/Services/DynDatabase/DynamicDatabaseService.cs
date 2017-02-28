using OsmiumMine.Core.Services.Database;
using OsmiumMine.Core.Utilities;
using IridiumIon.JsonFlat3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsmiumMine.Core.Services.DynDatabase
{
    public class DynamicDatabaseService
    {
        public static async Task<string> PlaceData(JObject dataBundleRoot, string subDatabaseId, string path, NodeDataOvewriteMode ovewriteMode, bool response = true)
        {
            var domain = KeyValueDatabaseService.GetDomainPath(subDatabaseId);
            string result = null; // Optionally used to return a result
            var dataPath = new FlatJsonPath(path);
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
                switch (ovewriteMode)
                {
                    case NodeDataOvewriteMode.Update:
                        {
                            // Flatten input bundle
                            var flattenedBundle = new FlatJsonObject(dataBundleRoot, dataPath.TokenPrefix);
                            // Merge input bundle
                            foreach (var kvp in flattenedBundle.Dictionary)
                            {
                                KeyValueDatabaseService.Store.Set(domain, kvp.Key, kvp.Value);
                            }
                        }

                        break;

                    case NodeDataOvewriteMode.Put:
                        {
                            // Flatten input bundle
                            var dataTokenPrefix = dataPath.TokenPrefix;
                            // Get existing data
                            var flattenedBundle = new FlatJsonObject(dataBundleRoot, dataTokenPrefix);
                            var existingData = KeyValueDatabaseService.Store.GetKeys(domain).Where(x => x.StartsWith(dataTokenPrefix, StringComparison.Ordinal));
                            // Remove existing data
                            foreach (var key in existingData)
                            {
                                KeyValueDatabaseService.Store.Delete(domain, key);
                            }
                            // Add input bundle
                            foreach (var kvp in flattenedBundle.Dictionary)
                            {
                                KeyValueDatabaseService.Store.Set(domain, kvp.Key, kvp.Value);
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
                                KeyValueDatabaseService.Store.Set(domain, kvp.Key, kvp.Value);
                            }
                        }
                        break;
                }

                // Data was written
                if (response) result = dataBundleRoot.ToString();
            });
            return result;
        }

        public static async Task DeleteData(string subDatabaseId, string path)
        {
            var domain = KeyValueDatabaseService.GetDomainPath(subDatabaseId);
            var dataPath = new FlatJsonPath(path);
            await Task.Run(() =>
            {
                // Get existing data
                var existingData = KeyValueDatabaseService.Store.GetKeys(domain).Where(x => x.StartsWith(dataPath.TokenPrefix, StringComparison.Ordinal));
                // Remove existing data
                foreach (var key in existingData)
                {
                    KeyValueDatabaseService.Store.Delete(domain, key);
                }
            });
        }

        public static async Task<JToken> GetData(string subDatabaseId, string path, bool shallow = false)
        {
            var domain = KeyValueDatabaseService.GetDomainPath(subDatabaseId);
            var dataPath = new FlatJsonPath(path);
            return await Task.Run(() =>
            {
                if (!KeyValueDatabaseService.Store.Exists(domain)) return null; // 404 - There's no data container
                // Unflatten and retrieve JSON object
                var flatJsonDict = new Dictionary<string, string>();
                // Use an optimization to only fetch requested keys
                foreach (var key in KeyValueDatabaseService.Store.GetKeysEnumerable(domain).Where(x => x.StartsWith(dataPath.TokenPrefix, StringComparison.Ordinal)))
                {
                    var val = KeyValueDatabaseService.Store.Get(domain, key);
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