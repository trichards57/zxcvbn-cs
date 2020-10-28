namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match identified as a sequence of keys on a recognised keyboard.
    /// </summary>
    public class SpatialMatch : Match
    {
        /// <summary>
        /// Gets The name of the keyboard layout used to make the spatial match.
        /// </summary>
        public string Graph { get; internal set; }

        /// <summary>
        /// Gets the name of the pattern matcher used to generate this match.
        /// </summary>
        public override string Pattern => "spatial";

        /// <summary>
        /// Gets the number of shifted characters matched in the pattern.
        /// </summary>
        public int ShiftedCount { get; internal set; }

        /// <summary>
        /// Gets the number of turns made.
        /// </summary>
        public int Turns { get; internal set; }
    }
}
