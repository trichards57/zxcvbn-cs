using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// This matcher applies some known l33t character substitutions and then attempts to match against passed in dictionary matchers.
    /// This detects passwords like 4pple which has a '4' substituted for an 'a'
    /// </summary>
    public class L33tMatcher : IMatcher
    {
        internal static ReadOnlyDictionary<char, char[]> L33tTable = new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>
        {
            ['a'] = new[] { '4', '@' },
            ['b'] = new[] { '8' },
            ['c'] = new[] { '(', '{', '[', '<' },
            ['e'] = new[] { '3' },
            ['g'] = new[] { '6', '9' },
            ['i'] = new[] { '1', '!', '|' },
            ['l'] = new[] { '1', '|', '7' },
            ['o'] = new[] { '0' },
            ['s'] = new[] { '$', '5' },
            ['t'] = new[] { '+', '7' },
            ['x'] = new[] { '%' },
            ['z'] = new[] { '2' }
        });

        private readonly List<DictionaryMatcher> _dictionaryMatchers;

        /// <summary>
        /// Create a l33t matcher that applies substitutions and then matches agains the passed in list of dictionary matchers.
        /// </summary>
        /// <param name="dictionaryMatchers">The list of dictionary matchers to check transformed passwords against</param>
        public L33tMatcher(List<DictionaryMatcher> dictionaryMatchers)
        {
            _dictionaryMatchers = dictionaryMatchers;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a l33t matcher that applies substitutions and then matches agains a single dictionary matcher.
        /// </summary>
        /// <param name="dictionaryMatcher">The dictionary matcher to check transformed passwords against</param>
        public L33tMatcher(DictionaryMatcher dictionaryMatcher) : this(new List<DictionaryMatcher> { dictionaryMatcher })
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Apply applicable l33t transformations and check <paramref name="password" /> against the dictionaries.
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>A list of match objects where l33t substitutions match dictionary words</returns>
        /// <seealso cref="T:Zxcvbn.Matcher.L33tDictionaryMatch" />
        public IEnumerable<Match> MatchPassword(string password)
        {
            var result = new List<DictionaryMatch>();

            foreach (var sub in EnumerateSubtitutions(RelevantL33tSubtable(password)))
            {
                if (!sub.Any())
                    break;

                var subbedPassword = TranslateString(sub, password);

                foreach (var matcher in _dictionaryMatchers)
                {
                    foreach (DictionaryMatch match in matcher.MatchPassword(subbedPassword))
                    {
                        var token = password.Substring(match.i, match.j - match.i + 1);
                        if (token.Equals(match.MatchedWord, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        var matchSub = new Dictionary<char, char>();

                        foreach (var subbedChar in sub.Keys)
                        {
                            var chr = sub[subbedChar];
                            if (token.Contains(subbedChar))
                                matchSub[subbedChar] = chr;
                        }

                        match.L33t = true;
                        match.Token = token;
                        match.Sub = matchSub;

                        result.Add(match);
                    }
                }
            }

            return result.Where(m => m.Token.Length > 1).OrderBy(m => m.i).ThenBy(m => m.j);
        }

        internal static List<Dictionary<char, char>> EnumerateSubtitutions(ReadOnlyDictionary<char, char[]> table)
        {
            return Helper(table.Keys, table).Select(s =>
            {
                var subDictionary = new Dictionary<char, char>();
                foreach (var item in s)
                {
                    var l33tChar = item.Item1;
                    var chr = item.Item2;
                    subDictionary[l33tChar] = chr;
                }
                return subDictionary;
            }).ToList();
        }

        internal static ReadOnlyDictionary<char, char[]> RelevantL33tSubtable(string password)
        {
            var subtable = new Dictionary<char, char[]>();

            foreach (var c in L33tTable.Keys)
            {
                var relevantSubs = L33tTable[c].Where(s => password.Contains(s));
                if (relevantSubs.Any())
                    subtable[c] = relevantSubs.ToArray();
            }

            return new ReadOnlyDictionary<char, char[]>(subtable);
        }

        private static List<List<Tuple<char, char>>> Deduplicate(List<List<Tuple<char, char>>> subs)
        {
            var result = new List<List<Tuple<char, char>>>();
            var members = new HashSet<string>();

            foreach (var sub in subs)
            {
                var label = string.Join("-", sub
                    .Select((kvp, i) => new Tuple<Tuple<char, char>, int>(kvp, i))
                    .OrderBy(i => i.ToString())
                    .Select((kvp, i) => kvp.ToString()));

                if (!members.Contains(label))
                {
                    members.Add(label);
                    result.Add(sub);
                }
            }

            return result;
        }

        private static List<List<Tuple<char, char>>> Helper(IEnumerable<char> keys, ReadOnlyDictionary<char, char[]> table, List<List<Tuple<char, char>>> subs = null)
        {
            if (subs == null)
            {
                subs = new List<List<Tuple<char, char>>>
                {
                    new List<Tuple<char, char>>()
                };
            }

            if (!keys.Any())
                return subs;

            var firstKey = keys.First();
            var restOfKeys = keys.Skip(1);

            var nextSubs = new List<List<Tuple<char, char>>>();

            foreach (var l33tChr in table[firstKey])
            {
                foreach (var sub in subs)
                {
                    var dupL33tIndex = sub.FindIndex(s => s.Item1 == l33tChr);

                    if (dupL33tIndex != -1)
                        nextSubs.Add(sub);

                    nextSubs.Add(new List<Tuple<char, char>>(sub)
                    {
                        new Tuple<char, char>(l33tChr, firstKey)
                    });
                }
            }

            subs = Deduplicate(nextSubs);
            return Helper(restOfKeys, table, subs);
        }

        private static string TranslateString(IReadOnlyDictionary<char, char> charMap, string str)
        {
            // Make substitutions from the character map wherever possible
            return new string(str.Select(c => charMap.ContainsKey(c) ? charMap[c] : c).ToArray());
        }

        private class SubstituionComparer : IEqualityComparer<Tuple<char, char>>
        {
            public bool Equals(Tuple<char, char> x, Tuple<char, char> y)
            {
                return x.Item1 == y.Item1 && x.Item2 == y.Item2;
            }

            public int GetHashCode(Tuple<char, char> obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
