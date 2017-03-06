using Nancy;
using System.Collections.Generic;
using System.Security.Claims;

namespace OsmiumMine.Core.Server.Services.Authentication.Security
{
    public static class ApiAccessModuleSecurityExtensions
    {
        /// <summary>
        /// Ensures that the user either has all the claims in the collection, or has the admin (override) claim
        /// </summary>
        /// <param name="module"></param>
        /// <param name="claims"></param>
        /// <param name="adminClaim"></param>
        public static void RequiresAllClaims(this NancyModule module, IEnumerable<Claim> claims, Claim adminClaim = null)
        {
            module.Before.AddItemToEndOfPipeline((ctx) =>
            {
                if (ctx.CurrentUser == null)
                {
                    return HttpStatusCode.Unauthorized;
                }
                return ctx.CurrentUser.EnsureAllClaims(claims, adminClaim) ? null : (Response)HttpStatusCode.Unauthorized;
            });
        }

        public static bool EnsureAllClaims(this ClaimsPrincipal principal, IEnumerable<Claim> claims, Claim adminClaim = null)
        {
            // Override claim
            if (adminClaim != null && principal.EnsureClaim(adminClaim))
            {
                return true;
            }
            // Make sure all claims are available
            foreach (var testClaim in claims)
            {
                if (!principal.EnsureClaim(testClaim))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Ensures that a ClaimsPrincipal posesses a claim by checking the Type and Value fields
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static bool EnsureClaim(this ClaimsPrincipal principal, Claim claim)
        {
            return principal.HasClaim(claim.Type, claim.Value);
        }
    }
}