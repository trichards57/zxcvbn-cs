using System.Collections.Generic;

namespace Zxcvbn.Matcher.Matches
{
    /// <inheritdoc />
    /// <summary>
    /// A match found with the RepeatMatcher
    /// </summary>
    public class RepeatMatch : Match
    {
        public double BaseGuesses { get; set; }

        public List<Match> BaseMatches { get; set; }

        public string BaseToken { get; set; }

        /// <summary>
        /// The character that was repeated
        /// </summary>
        public char RepeatChar { get; set; }

        public int RepeatCount { get; set; }
    }
}
