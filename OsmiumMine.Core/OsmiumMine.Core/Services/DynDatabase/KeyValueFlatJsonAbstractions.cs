using OsmiumMine.Core.Services.Database;

namespace OsmiumMine.Core.Services.DynDatabase
{
    public class KeyValueFlatJsonAbstractions
    {
        public static bool HasRootObject(KeyValueDatabaseService databaseService, string subDatabaseId)
        {
            //return KeyValueDatabaseService.Store.Get(KeyValueDatabaseService.GetDomainPath(subDatabaseId, false), KeyValueDatabaseService.GetDSPrefixKey(subDatabaseId, false)) == "1";
            return databaseService.Store.Exists(databaseService.GetDomainPath(subDatabaseId));
        }

        public static bool CreateRootObject(KeyValueDatabaseService databaseService, string subDatabaseId)
        {
            //return KeyValueDatabaseService.Store.Set(KeyValueDatabaseService.GetDomainPath(subDatabaseId, false), "", "1");
            return databaseService.Store.Exists(databaseService.GetDomainPath(subDatabaseId));
        }
    }
}