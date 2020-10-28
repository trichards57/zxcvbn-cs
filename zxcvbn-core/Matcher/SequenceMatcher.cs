using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Attempts to match a string with a sequence of characters.
    /// </summary>
    internal class SequenceMatcher : IMatcher
    {
        private const int MaxDelta = 5;

        /// <summary>
        /// Find sequence matches in <paramref name="password" />.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>An enumerable of sequence matches.</returns>
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            if (password.Length <= 1)
                return Enumerable.Empty<Matches.Match>();

            var result = new List<Matches.Match>();

            void Update(int i, int j, int delta)
            {
                if (j - i > 1 || Math.Abs(delta) == 1)
                {
                    if (Math.Abs(delta) > 0 && Math.Abs(delta) <= MaxDelta)
                    {
                        var token = password.Substring(i, j - i + 1);
                        string sequenceName;
                        int sequenceSpace;

                        if (Regex.IsMatch(token, "^[a-z]+$"))
                        {
                            sequenceName = "lower";
                            sequenceSpace = 26;
                        }
                        else if (Regex.IsMatch(token, "^[A-Z]+$"))
                        {
                            sequenceName = "upper";
                            sequenceSpace = 26;
                        }
                        else if (Regex.IsMatch(token, "^\\d+$"))
                        {
                            sequenceName = "digits";
                            sequenceSpace = 10;
                        }
                        else
                        {
                            sequenceName = "unicode";
                            sequenceSpace = 26;
                        }

                        result.Add(new SequenceMatch
                        {
                            i = i,
                            j = j,
                            Token = token,
                            SequenceName = sequenceName,
                            SequenceSpace = sequenceSpace,
                            Ascending = delta > 0,
                        });
                    }
                }
            }

            var iIn = 0;
            int? lastDelta = null;

            for (var k = 1; k < password.Length; k++)
            {
                var deltaIn = password[k] - password[k - 1];
                if (lastDelta == null)
                    lastDelta = deltaIn;
                if (deltaIn == lastDelta)
                    continue;

                var jIn = k - 1;
                Update(iIn, jIn, lastDelta.Value);
                iIn = jIn;
                lastDelta = deltaIn;
            }

            Update(iIn, password.Length - 1, lastDelta.Value);
            return result;
        }
    }
}
