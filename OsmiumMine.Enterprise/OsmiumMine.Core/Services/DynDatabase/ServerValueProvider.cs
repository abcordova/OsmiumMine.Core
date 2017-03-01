using Newtonsoft.Json.Linq;
using System;

namespace OsmiumMine.Core.Services.DynDatabase
{
    /// <summary>
    /// Provides implementations for server values
    /// </summary>
    public static class ServerValueProvider
    {
        public static string ServerValueKeyName => ".sv";

        public static JValue GetValue(string serverValueName)
        {
            switch (serverValueName)
            {
                case "timestamp":
                    return new JValue(DateTimeOffset.Now.ToUnixTimeSeconds());

                default:
                    {
                        return JValue.CreateNull();
                    }
            }
        }
    }
}