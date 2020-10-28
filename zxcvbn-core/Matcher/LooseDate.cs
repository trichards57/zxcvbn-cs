namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Represents a possible date without the limitations of DateTime.
    /// </summary>
    internal struct LooseDate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LooseDate"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        public LooseDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        /// <summary>
        /// Gets the day in the date.
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// Gets the month in the date.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Gets the year in the date.
        /// </summary>
        public int Year { get; }
    }
}
