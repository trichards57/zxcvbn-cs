namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match identified as a possible date.
    /// </summary>
    /// <seealso cref="Zxcvbn.Matcher.Matches.Match" />
    public class DateMatch : Match
    {
        /// <summary>
        /// Gets the detected day.
        /// </summary>
        public int Day { get; internal set; }

        /// <summary>
        /// Gets the detected month.
        /// </summary>
        public int Month { get; internal set; }

        public override string Pattern => "date";

        /// <summary>
        /// Gets the separator detected in the date.
        /// </summary>
        /// <remarks>
        /// If there is no separator then this will be an empty string.
        /// </remarks>
        public string Separator { get; internal set; }

        /// <summary>
        /// Gets the detected year.
        /// </summary>
        public int Year { get; internal set; }
    }
}
