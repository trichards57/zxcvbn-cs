using System.Collections.Generic;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// Represents the most guessable match provided.
    /// </summary>
    internal class MostGuessableMatchResult
    {
        /// <summary>
        /// Gets or sets the guesses the match is estimated to need.
        /// </summary>
        public long Guesses { get; set; }

        /// <summary>
        /// Gets or sets the password being assessed.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the score associated with the most guessable match.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the matches identified.
        /// </summary>
        public IEnumerable<Match> Sequence { get; set; }
    }
}
