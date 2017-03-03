using OsmiumMine.Core.Server.Configuration;
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

        public ClaimsPrincipal ResolveClientIdentity(string apiKey)
        {
            var currentKey = ServerContext.ServerState.ApiKeys.FirstOrDefault(x => x.Key == apiKey);
            if (currentKey != null)
            {
                // Give client identity
                var accessValidator = new ClientApiAccessValidator();
                var keyAuthClaims = accessValidator.GetAuthClaims(currentKey);
                return new ClaimsPrincipal(new ClaimsIdentity(keyAuthClaims));
                //return new ClaimsPrincipal(new GenericIdentity("data client", "stateless"));
            }
            return null;
        }
    }
}