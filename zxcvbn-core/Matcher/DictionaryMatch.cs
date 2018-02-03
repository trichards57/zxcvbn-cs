namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// Matches found by the dictionary matcher contain some additional information about the matched word.
    /// </summary>
    public class DictionaryMatch : Match
    {
        /// <summary>
        /// The base entropy of the match, calculated from frequency rank
        /// </summary>
        public double BaseEntropy { get; set; }

        /// <summary>
        /// The name of the dictionary the matched word was found in
        /// </summary>
        public string DictionaryName { get; set; }

        /// <summary>
        /// The dictionary word matched
        /// </summary>
        public string MatchedWord { get; set; }

        /// <summary>
        /// The rank of the matched word in the dictionary (i.e. 1 is most frequent, and larger numbers are less common words)
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Additional entropy for this match from the use of mixed case
        /// </summary>
        public double UppercaseEntropy { get; set; }
    }
}