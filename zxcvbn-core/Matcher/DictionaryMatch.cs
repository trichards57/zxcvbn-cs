namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// Matches found by the dictionary matcher contain some additional information about the matched word.
    /// </summary>
    public class DictionaryMatch : Match
    {
        public DictionaryMatch()
        {
            Pattern = DictionaryMatcher.DictionaryPattern;
        }

        /// <summary>
        /// The base entropy of the match, calculated from frequency rank
        /// </summary>
        public double BaseEntropy { get; set; }

        public long BaseGuesses { get; set; }

        /// <summary>
        /// The name of the dictionary the matched word was found in
        /// </summary>
        public string DictionaryName { get; set; }

        /// <summary>
        /// The matched word was found with l33t spelling
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool L33t { get; set; }

        // ReSharper disable once InconsistentNaming
        public long L33tVariations { get; set; }

        /// <summary>
        /// The dictionary word matched
        /// </summary>
        public string MatchedWord { get; set; }

        /// <summary>
        /// The rank of the matched word in the dictionary (i.e. 1 is most frequent, and larger numbers are less common words)
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// The matched word was reversed compared to the dictionary
        /// </summary>
        public bool Reversed { get; set; }

        /// <summary>
        /// Additional entropy for this match from the use of mixed case
        /// </summary>
        public double UppercaseEntropy { get; set; }

        public long UppercaseVariations { get; set; }
        public long Variations { get; set; }
    }
}