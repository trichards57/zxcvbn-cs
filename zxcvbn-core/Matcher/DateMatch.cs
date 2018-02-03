namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// A match found by the date matcher
    /// </summary>
    public class DateMatch : Match
    {
        /// <summary>
        /// The detected day
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// The detected month
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Where a date with separators is matched, this will contain the separator that was used (e.g. '/', '-')
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// The detected year
        /// </summary>
        public int Year { get; set; }
    }
}