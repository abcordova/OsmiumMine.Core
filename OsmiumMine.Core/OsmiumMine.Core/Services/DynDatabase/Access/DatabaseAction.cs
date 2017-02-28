using System;

namespace OsmiumMine.Core.Services.DynDatabase.Access
{
    [Flags]
    public enum DatabaseAction
    {
        Retrieve = 1 << 0,
        Delete = 1 << 1,
        Push = 1 << 2,
        Update = 1 << 3,
        Put = 1 << 4,
        Read = Retrieve,
        Write = Push | Update | Put,
        All = Retrieve | Delete | Push | Update | Put
    }
}