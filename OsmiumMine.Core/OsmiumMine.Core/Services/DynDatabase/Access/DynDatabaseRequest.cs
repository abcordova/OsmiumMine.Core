namespace OsmiumMine.Core.Services.DynDatabase.Access
{
    public class DynDatabaseRequest
    {
        public enum PermissionState
        {
            Granted,
            Denied
        }

        public string DatabaseId { get; set; }
        public string Path { get; set; }
        public bool Valid { get; set; }
        public PermissionState State { get; set; }
        public DatabaseAction RequestedAction { get; set; }
    }
}