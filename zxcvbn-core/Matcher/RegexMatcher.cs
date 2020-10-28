using System.Collections.Generic;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// <para>Use a regular expression to match agains the password. (e.g. 'year' and 'digits' pattern matchers are implemented with this matcher.</para>
    /// <para>A note about cardinality: the cardinality parameter is used to calculate the entropy of matches found with the regex matcher. Since
    /// this cannot be calculated automatically from the regex pattern it must be provided. It can be provided per-character or per-match. Per-match will
    /// result in every match having the same entropy (lg cardinality) whereas per-character will depend on the match length (lg cardinality ^ length)</para>
    /// </summary>
    internal class RegexMatcher : IMatcher
    {
        private readonly string _matcherName;
        private readonly Regex _matchRegex;

        /// <inheritdoc />
        /// <summary>
        /// Create a new regex pattern matcher
        /// </summary>
        /// <param name="pattern">The regex pattern to match</param>
        /// <param name="matcherName">The name to give this matcher ('pattern' in resulting matches)</param>
        public RegexMatcher(string pattern, string matcherName = "regex")
            : this(new Regex(pattern), matcherName)
        {
        }

        /// <summary>
        /// Create a new regex pattern matcher
        /// </summary>
        /// <param name="matchRegex">The regex object used to perform matching</param>
        /// <param name="matcherName">The name to give this matcher ('pattern' in resulting matches)</param>
        public RegexMatcher(Regex matchRegex, string matcherName = "regex")
        {
            _matchRegex = matchRegex;
            _matcherName = matcherName;
        }

        public static string RegexPattern { get; } = "regex";

        /// <inheritdoc />
        /// <summary>
        /// Find all matches of the regex in <paramref name="password" />
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>An enumerable of matches for each regex match in <paramref name="password" /></returns>
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            var reMatches = _matchRegex.Matches(password);

            var pwMatches = new List<Matches.Match>();

            foreach (System.Text.RegularExpressions.Match rem in reMatches)
            {
                pwMatches.Add(new RegexMatch
                {
                    Pattern = RegexPattern,
                    RegexName = _matcherName,
                    i = rem.Index,
                    j = rem.Index + rem.Length - 1,
                    Token = password.Substring(rem.Index, rem.Length)
                });
            }

            return pwMatches;
        }
    }
}
