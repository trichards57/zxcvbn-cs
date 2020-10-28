namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match identified as a predictable sequence of characters.
    /// </summary>
    public class SequenceMatch : Match
    {
        /// <summary>
        /// Gets a value indicating whether the match was found in ascending order (cdefg) or not (zyxw).
        /// </summary>
        public bool Ascending { get; internal set; }

        /// <summary>
        /// Gets the name of the pattern matcher used to generate this match.
        /// </summary>
        public override string Pattern => "sequence";

        /// <summary>
        /// Gets the name of the sequence that the match was found in.
        /// </summary>
        public string SequenceName { get; internal set; }

        /// <summary>
        /// Gets the size of the sequence the pattern was found in.
        /// </summary>
        public int SequenceSpace { get; internal set; }
    }
}
