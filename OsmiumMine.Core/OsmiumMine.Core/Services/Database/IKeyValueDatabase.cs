using System.Collections.Generic;

namespace OsmiumMine.Core.Services.Database
{
    public interface IKeyValueDatabase
    {
        bool Exists(string key);

        bool Set(string key, string value);

        bool Set(string domain, string key, string value);

        bool Delete(string key);

        bool Delete(string domain, string key);

        string Get(string key);

        string Get(string domain, string key);

        string[] GetKeys(string domain);

        IEnumerable<string> GetKeysEnumerable(string domain);
    }
}