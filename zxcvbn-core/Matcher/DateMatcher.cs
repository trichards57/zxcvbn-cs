using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// <para>This matcher attempts to guess dates, with and without date separators. e.g. 1197 (could be 1/1/97) through to 18/12/2015.</para>
    /// <para>The format for matching dates is quite particular, and only detected years in the range 00-99 and 1900-2019 are considered by
    /// this matcher.</para>
    /// </summary>
    public class DateMatcher : IMatcher
    {
        // TODO: This whole matcher is a rather messy but works (just), could do with a touching up. In particular it does not provide matched date details for dates without separators

        private const string DatePattern = "date";
        private const int MaxYear = 2050;
        private const int MinYear = 1000;

        private readonly Dictionary<int, int[][]> _dateSplits = new Dictionary<int, int[][]>
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

        private readonly Regex _dateWithNoSeperater = new Regex("^\\d{4,8}$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        // The two regexes for matching dates with slashes is lifted directly from zxcvbn (matching.coffee about :400)
        private readonly Regex _dateWithSeperator = new Regex(
            @"^( \d{1,4} )    # day or month
               ( [\s/\\_.-] ) # separator
               ( \d{1,2} )    # month or day
               \2             # same separator
               ( \d{1,4} )    # year
              $", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private readonly int _referenceYear = DateTime.Now.Year;

        /// <inheritdoc />
        /// <summary>
        /// Find date matches in <paramref name="password" />
        /// </summary>
        /// <param name="password">The passsord to check</param>
        /// <returns>An enumerable of date matches</returns>
        /// <seealso cref="T:Zxcvbn.Matcher.DateMatch" />
        public IEnumerable<Match> MatchPassword(string password)
        {
            var matches = new List<Match>();

            for (var i = 0; i <= password.Length - 4; i++)
            {
                for (var j = 4; i + j <= password.Length; j++)
                {
                    var dateMatch = _dateWithNoSeperater.Match(password); // Slashless dates
                    if (!dateMatch.Success)
                        continue;

                    var candidates = new List<LooseDate>();

                    foreach (var split in _dateSplits[dateMatch.Length])
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

                    int Metric(LooseDate c) => Math.Abs(c.Year - _referenceYear);

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
                    var match = _dateWithSeperator.Match(token);

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

        private static double CalculateEntropy(string match, int? year, bool separator)
        {
            // The entropy calculation is pretty straightforward

            // This is a slight departure from the zxcvbn case where the match has the actual year so the two-year vs four-year
            //   can always be known rather than guessed for strings without separators.
            if (!year.HasValue)
            {
                // Guess year length from string length
                year = match.Length <= 6 ? 99 : 9999;
            }

            var entropy = year < 100 ? Math.Log(31 * 12 * 100, 2) : Math.Log(31 * 12 * 119, 2);

            if (separator) entropy += 2; // Extra two bits for separator (/\...)

            return entropy;
        }

        private static LooseDate? MapIntsToDate(IReadOnlyList<int> vals)
        {
            if (vals[1] > 31 || vals[1] < 1)
                return null;

            var over12 = 0;
            var over31 = 0;
            var under1 = 0;

            foreach (var i in vals)
            {
                if (99 < i && i < MinYear || i > MaxYear)
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
                    return null;
                }

                foreach (var possibleSplit in possibleSplits)
                {
                    var dayMonth = MapIntsToDayMonth(new[] { possibleSplit[1], possibleSplit[2] });
                    if (dayMonth == null) continue;
                    var year = TwoToFourDigitYear(possibleSplit[0]);
                    return new LooseDate(year, dayMonth.Value.Month, dayMonth.Value.Day);
                }
            }

            return null;
        }

        private static LooseDate? MapIntsToDayMonth(IReadOnlyList<int> vals)
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