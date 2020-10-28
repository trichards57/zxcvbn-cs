namespace Zxcvbn.Matcher.Matches
{
    /// <inheritdoc />
    /// <summary>
    /// Matches found by the dictionary matcher contain some additional information about the matched word.
    /// </summary>
    internal class RegexMatch : Match
    {
        public RegexMatch()
        {
            Pattern = RegexMatcher.RegexPattern;
        }

        public string RegexName { get; set; }
    }
}
