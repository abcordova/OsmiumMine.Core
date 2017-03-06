using Newtonsoft.Json;

namespace OsmiumMine.Core.Services.DynDatabase.Access
{
    public class DynDatabaseRequest
    {
        public enum PermissionState
        {
            Granted,
            Denied
        }

        public string DatabaseId { get; internal set; }
        public string Path { get; internal set; }
        public string AuthToken { get; internal set; }
        public bool Valid { get; internal set; }
        public PermissionState State { get; internal set; }
        public DatabaseAction RequestedAction { get; set; }

        [JsonIgnore]
        public bool Granted => State == PermissionState.Granted;
    }
}