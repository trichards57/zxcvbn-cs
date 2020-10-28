namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match used to evaluate the brute-force strength of a password.
    /// </summary>
    /// <seealso cref="Zxcvbn.Matcher.Matches.Match" />
    public class BruteForceMatch : Match
    {
        /// <summary>
        /// Gets the name of the pattern matcher used to generate this match.
        /// </summary>
        public override string Pattern => "bruteforce";
    }
}
