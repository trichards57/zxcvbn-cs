using System.Collections.Generic;

namespace Zxcvbn.Matcher.Matches
{
    /// <inheritdoc />
    /// <summary>
    /// A match found with the RepeatMatcher
    /// </summary>
    internal class RepeatMatch : Match
    {
        public double BaseGuesses { get; set; }

        public List<Match> BaseMatches { get; set; }

        public string BaseToken { get; set; }

        public int RepeatCount { get; set; }
    }
}
