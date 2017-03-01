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
        Stream = 1 << 5,
        Read = Retrieve,
        ReadLive = Read | Stream,
        Write = Push | Update | Put,
        ReadWrite = Read | Write,
        All = ReadWrite | Stream
    }
}