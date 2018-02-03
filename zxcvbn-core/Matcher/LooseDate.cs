namespace Zxcvbn.Matcher
{
    internal struct LooseDate
    {
        public LooseDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public int Day { get; }
        public int Month { get; }
        public int Year { get; }
    }
}