using System.Collections.Generic;

namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match identified as containing some repeated details.
    /// </summary>
    public class RepeatMatch : Match
    {
        /// <summary>
        /// Gets the base number guesses associated with the base matches.
        /// </summary>
        public long BaseGuesses { get; internal set; }

        /// <summary>
        /// Gets the base matches that are repeated.
        /// </summary>
        public IReadOnlyList<Match> BaseMatches => BaseMatchItems.AsReadOnly();

        /// <summary>
        /// Gets the base repeated token.
        /// </summary>
        public string BaseToken { get; internal set; }

        /// <summary>
        /// Gets the name of the pattern matcher used to generate this match.
        /// </summary>
        public override string Pattern => "repeat";

        /// <summary>
        /// Gets the number of times the base token is repeated.
        /// </summary>
        public int RepeatCount { get; internal set; }

        /// <summary>
        ///  Gets or sets the base matches that are repeated.
        /// </summary>
        /// <remarks>
        /// The editable backing of BaseMatches.
        /// </remarks>
        internal List<Match> BaseMatchItems { get; set; } = new List<Match>();
    }
}
