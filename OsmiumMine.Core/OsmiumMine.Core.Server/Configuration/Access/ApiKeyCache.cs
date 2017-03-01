using System.Collections.Generic;
using System.Linq;

namespace OsmiumMine.Core.Server.Configuration.Access
{
    public class ApiKeyCache
    {
        public IEnumerable<ApiAccessKey> Keys { get; set; }
        public IEnumerable<string> KeyStrings { get; set; }

        public ApiKeyCache(IEnumerable<ApiAccessKey> keys)
        {
            Keys = keys;
            KeyStrings = Keys.Select(k => k.Key);
        }

        public ApiAccessKey FindKeyByKeyString(string keyString)
        {
            return Keys.FirstOrDefault(x => x.Key == keyString);
        }
    }
}