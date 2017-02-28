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
        public Regex PathRegex { get; private set; }

        public DatabaseAction AllowedActions { get; set; }

        public int Priority { get; }

        protected SecurityRule(Regex regex, DatabaseAction allowedActions)
        {
            AllowedActions = allowedActions;
            PathRegex = regex;
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