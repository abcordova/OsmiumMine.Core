using System.Collections.Generic;
using System.Linq;

namespace IridiumIon.JsonFlat3
{
    public class FlatJsonPath
    {
        public IList<string> Segments { get; }

        public FlatJsonPath(string uriPath)
        {
            Segments = uriPath.Split('/').ToList();
        }

        public string TokenPath
        {
            get
            {
                var joinedSegments = string.Join(".", Segments);
                return joinedSegments;
            }
        }

        public string TokenPrefix
        {
            get
            {
                var tokPath = TokenPath;
                // if tokPath isn't empty, it needs . suffix to be a valid prefix
                if (tokPath.Length > 0) tokPath += ".";
                return tokPath;
            }
        }
    }
}