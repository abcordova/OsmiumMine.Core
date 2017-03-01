using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IridiumIon.JsonFlat3.Internal
{
    /// <summary>
    /// Based on https://codedump.io/share/w4CmxtkbwSGD/1/how-to-unflatten-flattened-json-in-c
    /// </summary>
    internal class JsonUnflattenerHelper
    {
        private enum JsonKind
        {
            Object, Array
        }

        public static JObject UnflattenJObject(Dictionary<string, string> flattenedObj, bool shallow = false)
        {
            JContainer result = new JObject();
            var settings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge
            };
            foreach (var pathValue in flattenedObj)
            {
                if (shallow)
                {
                    //result.Merge(UnflattenSingle(
                    //    new KeyValuePair<string, string>(pathValue.Key, (pathValue.Value != null).ToString())),
                    //    settings);
                    var newContainer = new JObject(new JProperty(pathValue.Key, pathValue.Value != null));
                    result.Merge(newContainer, settings);
                }
                else
                {
                    result.Merge(UnflattenSingle(pathValue), settings);
                }
            }
            return result as JObject;
        }

        private static JContainer UnflattenSingle(KeyValuePair<string, string> keyValue)
        {
            // Get key
            string path = keyValue.Key;
            // Get value
            var rawValue = keyValue.Value;
            var pathSegments = SplitPathSegments(path);

            JContainer lastItem = null;
            // build from leaf to root
            foreach (var pathSegment in pathSegments.Reverse())
            {
                var type = GetJsonType(pathSegment);
                switch (type)
                {
                    case JsonKind.Object:
                        var obj = new JObject();
                        if (lastItem == null)
                        {
                            bool boolVal;
                            int intVal;
                            double doubleVal;
                            if (bool.TryParse(rawValue, out boolVal))
                            {
                                obj.Add(pathSegment, new JValue(boolVal));
                            }
                            if (int.TryParse(rawValue, out intVal))
                            {
                                obj.Add(pathSegment, new JValue(intVal));
                            }
                            else if (double.TryParse(rawValue, out doubleVal))
                            {
                                obj.Add(pathSegment, new JValue(doubleVal));
                            }
                            else
                            {
                                obj.Add(pathSegment, new JValue(rawValue));
                            }
                        }
                        else
                        {
                            obj.Add(pathSegment, lastItem);
                        }
                        lastItem = obj;
                        break;

                    case JsonKind.Array:
                        var array = new JArray();
                        var index = GetArrayIndex(pathSegment);
                        array = FillEmpty(array, index);
                        if (lastItem == null)
                        {
                            bool boolVal;
                            int intVal;
                            double doubleVal;
                            if (bool.TryParse(rawValue, out boolVal))
                            {
                                array[index] = new JValue(boolVal);
                            }
                            if (int.TryParse(rawValue, out intVal))
                            {
                                array[index] = new JValue(intVal);
                            }
                            else if (double.TryParse(rawValue, out doubleVal))
                            {
                                array[index] = new JValue(doubleVal);
                            }
                            else
                            {
                                array[index] = new JValue(rawValue);
                            }
                        }
                        else
                        {
                            array[index] = lastItem;
                        }
                        lastItem = array;
                        break;
                }
            }
            return lastItem;
        }

        private static IList<string> SplitPathSegments(string path)
        {
            var result = new List<string>();
            var reg = new Regex(@"(?!\.)([^. ^\[\]]+)|(?!\[)(\d+)(?=\])");
            foreach (Match match in reg.Matches(path))
            {
                result.Add(match.Value);
            }
            return result;
        }

        private static JArray FillEmpty(JArray array, int index)
        {
            for (int i = 0; i <= index; i++)
            {
                array.Add(null);
            }
            return array;
        }

        private static JsonKind GetJsonType(string pathSegment)
        {
            int x;
            return int.TryParse(pathSegment, out x) ? JsonKind.Array : JsonKind.Object;
        }

        private static int GetArrayIndex(string pathSegment)
        {
            int result;
            if (int.TryParse(pathSegment, out result))
            {
                return result;
            }
            throw new FormatException("Unable to parse array index: " + pathSegment);
        }
    }
}