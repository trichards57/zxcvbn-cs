using System.Collections.Generic;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Attempts to match a string with a pre-defined regular expressions.
    /// </summary>
    internal class RegexMatcher : IMatcher
    {
        private readonly string matcherName;
        private readonly Regex matchRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// Create a new regex pattern matcher.
        /// </summary>
        /// <param name="pattern">The regex pattern to match.</param>
        /// <param name="matcherName">The name to give this matcher ('pattern' in resulting matches).</param>
        public RegexMatcher(string pattern, string matcherName = "regex")
            : this(new Regex(pattern), matcherName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
        /// Create a new regex pattern matcher.
        /// </summary>
        /// <param name="matchRegex">The regex object used to perform matching.</param>
        /// <param name="matcherName">The name to give this matcher ('pattern' in resulting matches).</param>
        public RegexMatcher(Regex matchRegex, string matcherName = "regex")
        {
            this.matchRegex = matchRegex;
            this.matcherName = matcherName;
        }

        /// <summary>
        /// Find regex matches in <paramref name="password" />.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>An enumerable of regex matches.</returns>
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            var reMatches = matchRegex.Matches(password);

            var pwMatches = new List<Matches.Match>();

            foreach (System.Text.RegularExpressions.Match rem in reMatches)
            {
                pwMatches.Add(new RegexMatch
                {
                    RegexName = matcherName,
                    i = rem.Index,
                    j = rem.Index + rem.Length - 1,
                    Token = password.Substring(rem.Index, rem.Length),
                });
            }

            return pwMatches;
        }
    }
}
