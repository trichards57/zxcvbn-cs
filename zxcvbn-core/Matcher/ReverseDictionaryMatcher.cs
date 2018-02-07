using System.Collections.Generic;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    ///  <inheritdoc />
    public class ReverseDictionaryMatcher : DictionaryMatcher
    {
        /// <inheritdoc />
        public ReverseDictionaryMatcher(string name, string wordListPath) : base(name, wordListPath)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Match substrings of the reversed password agains the loaded dictionary
        /// </summary>
        public override IEnumerable<Match> MatchPassword(string password)
        {
            var matches = base.MatchPassword(password.StringReverse()).ToList();

            foreach (var m in matches)
            {
                if (!(m is DictionaryMatch ma))
                    continue;

                ma.Token = m.Token.StringReverse();
                ma.Reversed = true;
                ma.i = password.Length - 1 - m.j;
                ma.j = password.Length - 1 - m.i;
            }

            return matches.OrderBy(m => m.i).ThenBy(m => m.j);
        }
    }
}