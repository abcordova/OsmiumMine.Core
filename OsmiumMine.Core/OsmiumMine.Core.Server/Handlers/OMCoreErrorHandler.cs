using Nancy;
using Nancy.ErrorHandling;

namespace OsmiumMine.Core.Server.Handlers
{
    public class NotFoundHandler : IStatusCodeHandler
    {
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.Response = statusCode;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                   || statusCode == HttpStatusCode.NotImplemented;
        }
    }
}