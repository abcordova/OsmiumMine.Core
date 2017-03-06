using OsmiumMine.Core.Server.Configuration.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OsmiumMine.Core.Server.Services.Authentication
{
    public class ClientApiAccessValidator
    {
        public const string AuthTypeKey = "authType";
        public const string AccessScopeKey = "accessScope";

        /// <summary>
        /// Creates a list of claims based on the access scopes in the key
        /// </summary>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        public IEnumerable<Claim> GetAuthClaims(ApiAccessKey accessKey)
        {
            var claimList = new List<Claim>
            {
                new Claim(AuthTypeKey, RemoteAuthTypes.StatelessKey),
                new Claim(AccessScopeKey, accessKey.AccessScope.ToString())
            };
            return claimList;
        }

        /// <summary>
        /// Creates a claim from an access scope
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public Claim GetAccessClaim(ApiAccessScope scope)
        {
            return new Claim(AccessScopeKey, scope.ToString());
        }

        /// <summary>
        /// Retreives an access scope from a claim denoting access scope
        /// </summary>
        /// <param name="accessScopeClaim"></param>
        /// <returns></returns>
        public ApiAccessScope GetAccessScope(Claim accessScopeClaim)
        {
            return (ApiAccessScope)Enum.Parse(typeof(ApiAccessScope), accessScopeClaim.Value);
        }

        public Claim GetAccessScopeClaim(ApiAccessScope scope) => new Claim(AccessScopeKey, scope.ToString());
    }
}