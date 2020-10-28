using System.Collections.Generic;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Attempts to match a reversed string with a list of words.
    /// </summary>
    /// <seealso cref="Zxcvbn.Matcher.DictionaryMatcher" />
    internal class ReverseDictionaryMatcher : DictionaryMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseDictionaryMatcher"/> class.
        /// </summary>
        /// <param name="name">The name provided to the dictionary used.</param>
        /// <param name="wordListPath">The filename of the dictionary (full or relative path) or name of built-in dictionary.</param>
        public ReverseDictionaryMatcher(string name, string wordListPath)
            : base(name, wordListPath)
        {
        }

        /// <summary>
        /// Find reversed dictionary matches in <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>An enumerable of dictionary matches.</returns>
        public override IEnumerable<Match> MatchPassword(string password)
        {
            var matches = base.MatchPassword(password.StringReverse()).ToList();

            foreach (var m in matches)
            {
                if (!(m is DictionaryMatch ma))
                    continue;

                var i = ma.i;
                var j = ma.j;

                ma.Token = m.Token.StringReverse();
                ma.Reversed = true;
                ma.i = password.Length - 1 - j;
                ma.j = password.Length - 1 - i;
            }

            return matches.OrderBy(m => m.i).ThenBy(m => m.j);
        }
    }
}
