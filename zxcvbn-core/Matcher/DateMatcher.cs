using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

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

    /// <summary>
    /// <para>This matcher attempts to guess dates, with and without date separators. e.g. 1197 (could be 1/1/97) through to 18/12/2015.</para>
    ///
    /// <para>The format for matching dates is quite particular, and only detected years in the range 00-99 and 1900-2019 are considered by
    /// this matcher.</para>
    /// </summary>
    public class DateMatcher : IMatcher
    {
        // TODO: This whole matcher is a rather messy but works (just), could do with a touching up. In particular it does not provide matched date details for dates without separators

        private const string DatePattern = "date";
        private const int MaxYear = 2050;
        private const int MinYear = 1000;

        private readonly Dictionary<int, int[][]> DateSplits = new Dictionary<int, int[][]>
        {
            [4] = new[] {
                new[] { 1, 2 }, // 1 1 91
                new[] { 2, 3 }  // 91 1 1
            },
            [5] = new[]{
                new[] { 1, 3 }, // 1 11 91
                new[] { 2, 3 }  // 11 1 91
            },
            [6] = new[]{
                new[] { 1, 2 }, // 1 1 1991
                new[] { 2, 4 }, // 11 11 91
                new[] { 4, 5 }  // 1991 1 1
            },
            [7] = new[]{
                new[] { 1, 3 }, // 1 11 1991
                new[] { 2, 3 }, // 11 1 1991
                new[] { 4, 5 }, // 1991 1 11
                new[] { 4, 6 }  // 1991 11 1
            },
            [8] = new[] {
                new[] { 2, 4 }, // 11 11 1991
                new[] { 4, 6 }  // 1991 11 11
            }
        };

        private readonly Regex DateWithNoSeperater = new Regex("^\\d{4,8}$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        // The two regexes for matching dates with slashes is lifted directly from zxcvbn (matching.coffee about :400)
        private readonly Regex DateWithSeperator = new Regex(
            @"^( \d{1,4} )    # day or month
               ( [\s/\\_.-] ) # separator
               ( \d{1,2} )    # month or day
               \2             # same separator
               ( \d{1,4} )    # year
              $", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private readonly int ReferenceYear = DateTime.Now.Year;

        /// <summary>
        /// Find date matches in <paramref name="password"/>
        /// </summary>
        /// <param name="password">The passsord to check</param>
        /// <returns>An enumerable of date matches</returns>
        /// <seealso cref="DateMatch"/>
        public IEnumerable<Match> MatchPassword(string password)
        {
            var matches = new List<Match>();

            for (var i = 0; i <= password.Length - 4; i++)
            {
                for (var j = 4; i + j <= password.Length; j++)
                {
                    var dateMatch = DateWithNoSeperater.Match(password); // Slashless dates
                    if (!dateMatch.Success)
                        continue;

                    var candidates = new List<LooseDate>();

                    foreach (var split in DateSplits[dateMatch.Length])
                    {
                        var l = split[0];
                        var m = split[1];
                        var kLength = l;
                        var lLength = m - l;

                        var date = MapIntsToDate(new[] {
                                int.Parse(dateMatch.Value.Substring(0, kLength)),
                                int.Parse(dateMatch.Value.Substring(l, lLength)),
                                int.Parse(dateMatch.Value.Substring(m)) });

                        if (date != null)
                            candidates.Add(date.Value);
                    }

                    if (candidates.Count == 0)
                        continue;

                    var bestCandidate = candidates[0];

                    Func<LooseDate, int> metric = (LooseDate c) => Math.Abs(c.Year - ReferenceYear);

                    var minDistance = metric(bestCandidate);

                    foreach (var candidate in candidates.Skip(1))
                    {
                        var distance = metric(candidate);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            bestCandidate = candidate;
                        }
                    }

                    matches.Add(new DateMatch
                    {
                        Pattern = DatePattern,
                        Token = dateMatch.Value,
                        i = i,
                        j = j + i - 1,
                        Separator = "",
                        Year = bestCandidate.Year,
                        Month = bestCandidate.Month,
                        Day = bestCandidate.Day,
                        Entropy = CalculateEntropy(dateMatch.Value, bestCandidate.Year, true)
                    });
                }
            }

            for (var i = 0; i <= password.Length - 6; i++)
            {
                for (var j = 6; i + j <= password.Length; j++)
                {
                    var token = password.Substring(i, j);
                    var match = DateWithSeperator.Match(token);

                    if (!match.Success)
                        continue;

                    var date = MapIntsToDate(new[] {
                                int.Parse(match.Groups[1].Value),
                                int.Parse(match.Groups[3].Value),
                                int.Parse(match.Groups[4].Value) });

                    if (date == null)
                        continue;

                    var m = new DateMatch
                    {
                        Pattern = DatePattern,
                        Token = token,
                        i = i,
                        j = j + i - 1,
                        Separator = match.Groups[2].Value,
                        Year = date.Value.Year,
                        Month = date.Value.Month,
                        Day = date.Value.Day,
                        Entropy = CalculateEntropy(match.Value, date.Value.Year, true)
                    };

                    matches.Add(m);
                }
            }

            var filteredMatches = matches.Where(m =>
            {
                foreach (var n in matches)
                {
                    if (m == n)
                        continue;
                    if (n.i <= m.i && n.j >= m.j)
                        return false;
                }

                return true;
            });

            return filteredMatches;
        }

        private double CalculateEntropy(string match, int? year, bool separator)
        {
            // The entropy calculation is pretty straightforward

            // This is a slight departure from the zxcvbn case where the match has the actual year so the two-year vs four-year
            //   can always be known rather than guessed for strings without separators.
            if (!year.HasValue)
            {
                // Guess year length from string length
                year = match.Length <= 6 ? 99 : 9999;
            }

            var entropy = 0.0;
            if (year < 100) entropy = Math.Log(31 * 12 * 100, 2); // 100 years (two-digits)
            else entropy = Math.Log(31 * 12 * 119, 2); // 119 years (four digit years valid range)

            if (separator) entropy += 2; // Extra two bits for separator (/\...)

            return entropy;
        }

        /// <summary>
        /// Determine whether a string resembles a date (year first or year last)
        /// </summary>
        private bool IsDate(string match)
        {
            var isValid = false;

            // Try year length depending on match length. Length six should try both two and four digits

            if (match.Length <= 6)
            {
                // Try a two digit year, suffix and prefix
                isValid |= IsDateWithYearType(match, true, 2);
                isValid |= IsDateWithYearType(match, false, 2);
            }
            if (match.Length >= 6)
            {
                // Try a four digit year, suffix and prefix
                isValid |= IsDateWithYearType(match, true, 4);
                isValid |= IsDateWithYearType(match, false, 4);
            }

            return isValid;
        }

        private bool IsDateInRange(int year, int month, int day)
        {
            return IsYearInRange(year) && IsMonthDayInRange(month, day);
        }

        private bool IsDateWithYearType(string match, bool suffix, int yearLen)
        {
            var year = 0;
            if (suffix) match.IntParseSubstring(match.Length - yearLen, yearLen, out year);
            else match.IntParseSubstring(0, yearLen, out year);

            if (suffix) return IsYearInRange(year) && IsDayMonthString(match.Substring(0, match.Length - yearLen));
            else return IsYearInRange(year) && IsDayMonthString(match.Substring(yearLen, match.Length - yearLen));
        }

        /// <summary>
        /// Determines whether a substring of a date string resembles a day and month (day-month or month-day)
        /// </summary>
        private bool IsDayMonthString(string match)
        {
            int p1 = 0, p2 = 0;

            // Parse the day/month string into two parts
            if (match.Length == 2)
            {
                // e.g. 1 2 [1234]
                match.IntParseSubstring(0, 1, out p1);
                match.IntParseSubstring(1, 1, out p2);
            }
            else if (match.Length == 3)
            {
                // e.g. 1 12 [1234] or 12 1 [1234]

                match.IntParseSubstring(0, 1, out p1);
                match.IntParseSubstring(1, 2, out p2);

                // This one is a little different in that there's two ways to parse it so go one way first
                if (IsMonthDayInRange(p1, p2) || IsMonthDayInRange(p2, p1)) return true;

                match.IntParseSubstring(0, 2, out p1);
                match.IntParseSubstring(2, 1, out p2);
            }
            else if (match.Length == 4)
            {
                // e.g. 14 11 [1234]

                match.IntParseSubstring(0, 2, out p1);
                match.IntParseSubstring(2, 2, out p2);
            }

            // Check them both ways around to see if a valid day/month pair
            return IsMonthDayInRange(p1, p2) || IsMonthDayInRange(p2, p1);
        }

        // Assume all months have 31 days, we only care that things look like dates not that they're completely valid
        private bool IsMonthDayInRange(int month, int day)
        {
            return 1 <= month && month <= 12 && 1 <= day && day <= 31;
        }

        // Two-digit years are allowed, otherwise in 1900-2019
        private bool IsYearInRange(int year)
        {
            return (1900 <= year && year <= 2019) || (0 < year && year <= 99);
        }

        private LooseDate? MapIntsToDate(int[] vals)
        {
            if (vals[1] > 31 || vals[1] < 1)
                return null;

            var over12 = 0;
            var over31 = 0;
            var under1 = 0;

            foreach (var i in vals)
            {
                if ((99 < i && i < MinYear) || i > MaxYear)
                    return null;

                if (i > 31)
                    over31++;
                if (i > 12)
                    over12++;
                if (i < 1)
                    under1++;

                if (over31 >= 2 || over12 == 3 || under1 >= 2)
                    return null;

                var possibleSplits = new[]
                {
                    new[] {vals[2], vals[0], vals[1] },
                    new[] {vals[0], vals[1], vals[2] }
                };

                foreach (var possibleSplit in possibleSplits)
                {
                    if (possibleSplit[0] < MinYear || possibleSplit[0] > MaxYear)
                        continue;

                    var dayMonth = MapIntsToDayMonth(new[] { possibleSplit[1], possibleSplit[2] });
                    if (dayMonth != null)
                        return new LooseDate(possibleSplit[0], dayMonth.Value.Month, dayMonth.Value.Day);
                    else
                        return null;
                }

                foreach (var possibleSplit in possibleSplits)
                {
                    var dayMonth = MapIntsToDayMonth(new[] { possibleSplit[1], possibleSplit[2] });
                    if (dayMonth != null)
                    {
                        var year = TwoToFourDigitYear(possibleSplit[0]);
                        return new LooseDate(year, dayMonth.Value.Month, dayMonth.Value.Day);
                    }
                }
            }

            return null;
        }

        private LooseDate? MapIntsToDayMonth(int[] vals)
        {
            var day = vals[0];
            var month = vals[1];

            if (1 <= day && day <= 31 && 1 <= month && month <= 12)
                return new LooseDate(0, month, day);

            day = vals[1];
            month = vals[0];

            if (1 <= day && day <= 31 && 1 <= month && month <= 12)
                return new LooseDate(0, month, day);

            return null;
        }

        private int TwoToFourDigitYear(int year)
        {
            if (year > 99)
                return year;
            if (year > 50)
                return year + 1900;
            return year + 2000;
        }
    }
}