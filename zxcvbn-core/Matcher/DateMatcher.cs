using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Attempts to match a string with a date.
    /// </summary>
    /// <remarks>
    /// A date is recognised if it is:
    /// <list type="bullet">
    /// <item>Any 3 tuple that starts or ends with a 2- or 4- digit year</item>
    /// <item>With 2 or 0 separator characters</item>
    /// <item>May be zero padded</item>
    /// <item>Has a month between 1 and 12</item>
    /// <item>Has a day between 1 and 31</item>
    /// </list>
    ///
    /// This isn't true date parsing.  Invalid dates like 31 February will be allowed.
    /// </remarks>
    /// </summary>
    internal class DateMatcher : IMatcher
    {
        private const int MaxYear = 2050;
        private const int MinYear = 1000;

        private static readonly ReadOnlyDictionary<int, int[][]> DateSplits = new ReadOnlyDictionary<int, int[][]>(new Dictionary<int, int[][]>
        {
            [4] = new[]
            {
                new[] { 1, 2 }, // 1 1 91
                new[] { 2, 3 }, // 91 1 1
            },
            [5] = new[]
            {
                new[] { 1, 3 }, // 1 11 91
                new[] { 2, 3 }, // 11 1 91
            },
            [6] = new[]
            {
                new[] { 1, 2 }, // 1 1 1991
                new[] { 2, 4 }, // 11 11 91
                new[] { 4, 5 }, // 1991 1 1
            },
            [7] = new[]
            {
                new[] { 1, 3 }, // 1 11 1991
                new[] { 2, 3 }, // 11 1 1991
                new[] { 4, 5 }, // 1991 1 11
                new[] { 4, 6 }, // 1991 11 1
            },
            [8] = new[]
            {
                new[] { 2, 4 }, // 11 11 1991
                new[] { 4, 6 }, // 1991 11 11
            },
        });

        private readonly Regex dateWithNoSeperater = new Regex("^\\d{4,8}$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private readonly Regex dateWithSeperator = new Regex(
            @"^( \d{1,4} )    # day or month
               ( [\s/\\_.-] ) # separator
               ( \d{1,2} )    # month or day
               \2             # same separator
               ( \d{1,4} )    # year
              $", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Gets the reference year used to check for recent dates.
        /// </summary>
        public static int ReferenceYear { get; } = DateTime.Now.Year;

        /// <summary>
        /// Find date matches in <paramref name="password" />.
        /// </summary>
        /// <param name="password">The passsord to check.</param>
        /// <returns>An enumerable of date matches.</returns>
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            var matches = new List<Matches.Match>();

            for (var i = 0; i <= password.Length - 4; i++)
            {
                for (var j = 4; i + j <= password.Length; j++)
                {
                    var dateMatch = dateWithNoSeperater.Match(password); // Slashless dates
                    if (!dateMatch.Success)
                        continue;

                    var candidates = new List<LooseDate>();

                    foreach (var split in DateSplits[dateMatch.Length])
                    {
                        var l = split[0];
                        var m = split[1];
                        var kLength = l;
                        var lLength = m - l;

                        var date = MapIntsToDate(new[]
                        {
                                int.Parse(dateMatch.Value.Substring(0, kLength), CultureInfo.InvariantCulture),
                                int.Parse(dateMatch.Value.Substring(l, lLength), CultureInfo.InvariantCulture),
                                int.Parse(dateMatch.Value.Substring(m), CultureInfo.InvariantCulture),
                        });

                        if (date != null)
                            candidates.Add(date.Value);
                    }

                    if (candidates.Count == 0)
                        continue;

                    var bestCandidate = candidates[0];

                    int Metric(LooseDate c) => Math.Abs(c.Year - ReferenceYear);

                    var minDistance = Metric(bestCandidate);

                    foreach (var candidate in candidates.Skip(1))
                    {
                        var distance = Metric(candidate);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            bestCandidate = candidate;
                        }
                    }

                    matches.Add(new DateMatch
                    {
                        Token = dateMatch.Value,
                        i = i,
                        j = j + i - 1,
                        Separator = string.Empty,
                        Year = bestCandidate.Year,
                        Month = bestCandidate.Month,
                        Day = bestCandidate.Day,
                    });
                }
            }

            for (var i = 0; i <= password.Length - 6; i++)
            {
                for (var j = 6; i + j <= password.Length; j++)
                {
                    var token = password.Substring(i, j);
                    var match = dateWithSeperator.Match(token);

                    if (!match.Success)
                        continue;

                    var date = MapIntsToDate(new[]
                    {
                        int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                        int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                        int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                    });

                    if (date == null)
                        continue;

                    var m = new DateMatch
                    {
                        Token = token,
                        i = i,
                        j = j + i - 1,
                        Separator = match.Groups[2].Value,
                        Year = date.Value.Year,
                        Month = date.Value.Month,
                        Day = date.Value.Day,
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

        private static LooseDate? MapIntsToDate(IList<int> vals)
        {
            if (vals[1] > 31 || vals[1] < 1)
                return null;

            var over12 = 0;
            var over31 = 0;
            var under1 = 0;

            foreach (var i in vals)
            {
                if ((i > 99 && i < MinYear) || i > MaxYear)
                    return null;

                if (i > 31)
                    over31++;
                if (i > 12)
                    over12++;
                if (i < 1)
                    under1++;
            }

            if (over31 >= 2 || over12 == 3 || under1 >= 2)
                return null;

            var possibleSplits = new[]
            {
                new[] { vals[2], vals[0], vals[1] },
                new[] { vals[0], vals[1], vals[2] },
            };

            foreach (var possibleSplit in possibleSplits)
            {
                if (possibleSplit[0] < MinYear || possibleSplit[0] > MaxYear)
                    continue;

                var dayMonth = MapIntsToDayMonth(new[] { possibleSplit[1], possibleSplit[2] });
                if (dayMonth != null)
                    return new LooseDate(possibleSplit[0], dayMonth.Value.Month, dayMonth.Value.Day);
                return null;
            }

            foreach (var possibleSplit in possibleSplits)
            {
                var dayMonth = MapIntsToDayMonth(new[] { possibleSplit[1], possibleSplit[2] });
                if (dayMonth == null) continue;
                var year = TwoToFourDigitYear(possibleSplit[0]);
                return new LooseDate(year, dayMonth.Value.Month, dayMonth.Value.Day);
            }

            return null;
        }

        private static LooseDate? MapIntsToDayMonth(IList<int> vals)
        {
            var day = vals[0];
            var month = vals[1];

            if (day >= 1 && day <= 31 && month >= 1 && month <= 12)
                return new LooseDate(0, month, day);

            day = vals[1];
            month = vals[0];

            if (day >= 1 && day <= 31 && month >= 1 && month <= 12)
                return new LooseDate(0, month, day);

            return null;
        }

        private static int TwoToFourDigitYear(int year)
        {
            if (year > 99)
                return year;
            if (year > 50)
                return year + 1900;
            return year + 2000;
        }
    }
}
