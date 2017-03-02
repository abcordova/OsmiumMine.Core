using Newtonsoft.Json;
using OsmiumMine.Core.Services.DynDatabase.Access;
using OsmiumMine.Core.Utilities;
using System;
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
        public Regex PathRegex { get; set; }

        /// <summary>
        /// The actions that this rule applies to
        /// </summary>
        [JsonProperty("actions")]
        public DatabaseAction Actions { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("allow")]
        public bool Allow { get; set; }

        /// <summary>
        /// A unique identifier for the rule
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Creates a new security rule
        /// </summary>
        /// <param name="pathWildcard"></param>
        /// <param name="actions">The action flags to grant or restrict access to</param>
        /// <param name="allow">whether the rule should allow or deny</param>
        /// <param name="priority">If less than 0, the default priority calculation will be used</param>
        public SecurityRule(string pathWildcard, DatabaseAction actions = 0, bool allow = true, int priority = -1) : this(
                new Regex(WildcardMatcher.ToRegex(pathWildcard)), actions, allow, priority)
        {
        }

        /// <summary>
        /// Creates a new security rule
        /// </summary>
        /// <param name="pathRegex"></param>
        /// <param name="actions">The action flags to grant or restrict access to</param>
        /// <param name="allow">Whether the rule should allow or deny</param>
        /// <param name="priority">If less than 0, the default priority calculation will be used</param>
        public SecurityRule(Regex pathRegex, DatabaseAction actions = 0, bool allow = true, int priority = -1)
        {
            Actions = actions;
            PathRegex = pathRegex;
            Allow = allow;
            Id = Guid.NewGuid().ToString("N");

            // By default, use the pattern length for a rough approximation when the priority is not manually specified
            if (priority < 0)
            {
                Priority = PathRegex.ToString().Length;
            }
            else { Priority = priority; }
        }
    }
}