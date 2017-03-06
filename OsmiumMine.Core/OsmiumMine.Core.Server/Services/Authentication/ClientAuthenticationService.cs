using OsmiumMine.Core.Server.Configuration;
using OsmiumMine.Core.Server.Configuration.Access;
using System.Linq;
using System.Security.Claims;

namespace OsmiumMine.Core.Server.Services.Authentication
{
    public class ClientAuthenticationService
    {
        public IOMServerContext ServerContext { get; set; }

        public ClientAuthenticationService(IOMServerContext serverContext)
        {
            ServerContext = serverContext;
        }

        public ApiAccessKey ResolveKey(string apiKey)
        {
            return ServerContext.ServerState.ApiKeys.FirstOrDefault(x => x.Key == apiKey);
        }

        public ClaimsPrincipal ResolveClientIdentity(string apiKey)
        {
            var currentKey = ResolveKey(apiKey);
            if (currentKey != null)
            {
                // Give client identity
                var accessValidator = new ClientApiAccessValidator();
                var keyAuthClaims = accessValidator.GetAuthClaims(currentKey);
                return new ClaimsPrincipal(new ClaimsIdentity(keyAuthClaims));
            }
            return null;
        }
    }
}