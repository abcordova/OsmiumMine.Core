namespace OsmiumMine.Core.Services.DynDatabase.Access
{
    public class DynDatabaseRequest
    {
        public string DatabaseId { get; set; }
        public string Path { get; set; }
        public bool Valid { get; set; }
        public PermissionState PermissionState { get; set; }
    }
}