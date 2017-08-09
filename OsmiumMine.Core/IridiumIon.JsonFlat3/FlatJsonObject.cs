using System.Collections.Generic;
using System.Linq;
using IridiumIon.JsonFlat3.Internal;
using Newtonsoft.Json.Linq;

namespace IridiumIon.JsonFlat3
{
    /// <summary>
    ///     An abstraction representing a flattened JSON object
    /// </summary>
    public class FlatJsonObject
    {
        public FlatJsonObject() : this(new JObject())
        {
        }

        public FlatJsonObject(JObject jsonObject, string prefix = "")
        {
            Dictionary = JsonFlattener.FlattenJObject(jsonObject, prefix);
        }

        public FlatJsonObject(Dictionary<string, string> flattenedDict)
        {
            Dictionary = flattenedDict;
        }

        public Dictionary<string, string> Dictionary { get; }

        public void Merge(params FlatJsonObject[] other)
        {
            Dictionary.MergeInto(other.Select(o => o.Dictionary).ToArray());
        }

        public void RemoveNode(string path)
        {
            FlatJsonTools.RemoveNode(path, this);
        }

        public JObject Unflatten()
        {
            return JsonFlattener.UnflattenJObject(Dictionary);
        }

        public static implicit operator Dictionary<string, string>(FlatJsonObject instance)
        {
            return instance.Dictionary;
        }
    }
}