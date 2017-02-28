using OsmiumMine.Core.Server.Configuration;
using System.Security.Claims;

namespace OsmiumMine.Core.Server.Services.Authentication
{
    public class ClientAuthenticationService
    {
        public IOMCoreServerConfiguration Configuration { get; set; }

        public ClientAuthenticationService(IOMCoreServerConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ClaimsPrincipal ResolveClientIdentity(string apiKey)
        {
            var currentKey = Configuration.KeyCache.FindKeyByKeyString(apiKey);
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