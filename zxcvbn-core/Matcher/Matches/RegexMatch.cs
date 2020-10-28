namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match identified as matching one of the provided regular expressions.
    /// </summary>
    public class RegexMatch : Match
    {
        /// <summary>
        /// Gets the name of the pattern matcher used to generate this match.
        /// </summary>
        public override string Pattern => "regex";

        /// <summary>
        /// Gets the name of the regex that matched.
        /// </summary>
        public string RegexName { get; internal set; }
    }
}
