using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Services.DynDatabase
{
    public static class KeyValueFlatJsonAbstractions
    {
        public static bool HasRootObject(string subDatabaseId)
        {
            //return KeyValueDatabaseService.Store.Get(KeyValueDatabaseService.GetDomainPath(subDatabaseId, false), KeyValueDatabaseService.GetDSPrefixKey(subDatabaseId, false)) == "1";
            return KeyValueDatabaseService.Store.Exists(KeyValueDatabaseService.GetDomainPath(subDatabaseId));
        }

        public static bool CreateRootObject(string subDatabaseId)
        {
            //return KeyValueDatabaseService.Store.Set(KeyValueDatabaseService.GetDomainPath(subDatabaseId, false), "", "1");
            return KeyValueDatabaseService.Store.Exists(KeyValueDatabaseService.GetDomainPath(subDatabaseId));
        }
    }
}