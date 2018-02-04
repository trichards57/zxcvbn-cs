using System;
using System.Collections.Generic;
using System.Linq;

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
        public ReverseDictionaryMatcher(string name, IEnumerable<string> wordList) : base(name, wordList)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Match substrings of the reversed password agains the loaded dictionary
        /// </summary>
        public override IEnumerable<Match> MatchPassword(string password)
        {
            var matches = base.MatchPassword(password.StringReverse()).Cast<DictionaryMatch>().ToList();

            foreach (var m in matches)
            {
                m.Token = m.Token.StringReverse();
                m.Reversed = true;
                m.i = password.Length - 1 - m.j;
                m.j = password.Length - 1 - m.i;
            }

            return matches.OrderBy(m => m.i).ThenBy(m => m.j);
        }
    }
}