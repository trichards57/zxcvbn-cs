using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// Match repeated characters in the password (repeats must be more than two characters long to count)
    /// </summary>
    public class RepeatMatcher : IMatcher
    {
        private const string RepeatPattern = "repeat";

        /// <inheritdoc />
        /// <summary>
        /// Find repeat matches in <paramref name="password" />
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>List of repeat matches</returns>
        /// <seealso cref="T:Zxcvbn.Matcher.RepeatMatch" />
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            var matches = new List<Matches.Match>();
            var greedy = "(.+)\\1+";
            var lazy = "(.+?)\\1+";
            var lazyAnchored = "^(.+?)\\1+$";
            var lastIndex = 0;

            while (lastIndex < password.Length)
            {
                var greedyLastIndex = lastIndex;
                var lazyLastIndex = lastIndex;

                var greedyMatch = Regex.Match(password.Substring(greedyLastIndex), greedy);
                var lazyMatch = Regex.Match(password.Substring(lazyLastIndex), lazy);

                if (!greedyMatch.Success) break;

                System.Text.RegularExpressions.Match match;
                string baseToken;

                if (greedyMatch.Length > lazyMatch.Length)
                {
                    match = greedyMatch;
                    baseToken = Regex.Match(match.Value, lazyAnchored).Groups[1].Value;
                }
                else
                {
                    match = lazyMatch;
                    baseToken = match.Groups[1].Value;
                }

                var i = match.Index;
                var j = match.Index + match.Length - 1;

                var baseAnalysis =
                    PasswordScoring.MostGuessableMatchSequence(baseToken, Zxcvbn.GetAllMatches(baseToken));

                var baseMatches = baseAnalysis.Sequence;
                var baseGuesses = baseAnalysis.Guesses;

                matches.Add(new RepeatMatch
                {
                    Pattern = RepeatPattern,
                    i = i,
                    j = j,
                    Token = match.Value,
                    BaseToken = baseToken,
                    BaseGuesses = baseGuesses,
                    BaseMatches = baseMatches.ToList(),
                    RepeatCount = match.Length / baseToken.Length
                });

                lastIndex = j + 1;
            }

            return matches;
        }
    }
}
