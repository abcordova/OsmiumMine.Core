using Newtonsoft.Json;
using OsmiumMine.Core.Services.DynDatabase.Access;
using OsmiumMine.Core.Utilities;
using System.Text.RegularExpressions;

namespace OsmiumMine.Core.Services.Security
{
    public class SecurityRule
    {
        public enum Access
        {
            Allow,
            Deny
        }

        /// <summary>
        /// Compiled path regex
        /// </summary>
        [JsonProperty("pathRegex")]
        public Regex PathRegex { get; private set; }

        /// <summary>
        /// The actions that this rule applies to
        /// </summary>
        [JsonProperty("actions")]
        public DatabaseAction Actions { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; }

        [JsonProperty("allow")]
        public bool Allow { get; }

        protected SecurityRule(Regex pathRegex, DatabaseAction actions, bool allow = true)
        {
            Actions = actions;
            PathRegex = pathRegex;
            Allow = allow;
        }

        public static SecurityRule FromRegex(string regexPattern, DatabaseAction allowedActions = 0)
        {
            return new SecurityRule(new Regex(regexPattern, RegexOptions.Compiled), allowedActions);
        }

        public static SecurityRule FromWildcard(string wildcardPattern, DatabaseAction allowedActions = 0)
        {
            return FromRegex(WildcardMatcher.ToRegex(wildcardPattern), allowedActions);
        }
    }
}