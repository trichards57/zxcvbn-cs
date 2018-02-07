using System;
using System.Collections.Generic;

namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// This matcher applies some known l33t character substitutions and then attempts to match against passed in dictionary matchers.
    /// This detects passwords like 4pple which has a '4' substituted for an 'a'
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class L33tMatcher : IMatcher
    {
        //// ReSharper disable once InconsistentNaming
        //private static readonly ReadOnlyDictionary<char, char[]> _l33tTable = new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>
        //{
        //    ['a'] = new[] { '4', '@' },
        //    ['b'] = new[] { '8' },
        //    ['c'] = new[] { '(', '{', '[', '<' },
        //    ['e'] = new[] { '3' },
        //    ['g'] = new[] { '6', '9' },
        //    ['i'] = new[] { '1', '!', '|' },
        //    ['l'] = new[] { '1', '|', '7' },
        //    ['o'] = new[] { '0' },
        //    ['s'] = new[] { '$', '5' },
        //    ['t'] = new[] { '+', '7' },
        //    ['x'] = new[] { '%' },
        //    ['z'] = new[] { '2' }
        //});

        //private readonly List<DictionaryMatcher> _dictionaryMatchers;
        //private readonly Dictionary<char, char[]> _substitutions;

        ///// <summary>
        ///// Create a l33t matcher that applies substitutions and then matches agains the passed in list of dictionary matchers.
        ///// </summary>
        ///// <param name="dictionaryMatchers">The list of dictionary matchers to check transformed passwords against</param>
        //public L33tMatcher(List<DictionaryMatcher> dictionaryMatchers)
        //{
        //    _dictionaryMatchers = dictionaryMatchers;
        //    _substitutions = BuildSubstitutionsMap();
        //}

        ///// <inheritdoc />
        ///// <summary>
        ///// Create a l33t matcher that applies substitutions and then matches agains a single dictionary matcher.
        ///// </summary>
        ///// <param name="dictionaryMatcher">The dictionary matcher to check transformed passwords against</param>
        //public L33tMatcher(DictionaryMatcher dictionaryMatcher) : this(new List<DictionaryMatcher> { dictionaryMatcher })
        //{
        //}

        ///// <inheritdoc />
        ///// <summary>
        ///// Apply applicable l33t transformations and check <paramref name="password" /> against the dictionaries.
        ///// </summary>
        ///// <param name="password">The password to check</param>
        ///// <returns>A list of match objects where l33t substitutions match dictionary words</returns>
        ///// <seealso cref="T:Zxcvbn.Matcher.L33tDictionaryMatch" />
        //public IEnumerable<Match> MatchPassword(string password)
        //{
        //    var subs = EnumerateSubtitutions(GetRelevantSubstitutions(password));

        //    var matches = (from subDict in subs
        //                   let subPassword = TranslateString(subDict, password)
        //                   from matcher in _dictionaryMatchers
        //                   from match in matcher.MatchPassword(subPassword).OfType<DictionaryMatch>()
        //                   let token = password.Substring(match.i, match.j - match.i + 1)
        //                   let usedSubs = subDict.Where(kv => token.Contains(kv.Key)) // Count subs ised in matched token
        //                   where usedSubs.Any() // Only want matches where substitutions were used
        //                   select new L33tDictionaryMatch(match)
        //                   {
        //                       Token = token,
        //                       Subs = usedSubs.ToDictionary(kv => kv.Key, kv => kv.Value)
        //                   }).ToList();

        //    foreach (var match in matches) CalulateL33tEntropy(match);

        //    return matches;
        //}

        //private static Dictionary<char, char[]> BuildSubstitutionsMap()
        //{
        //    // Is there an easier way of building this table?
        //    var subs = new Dictionary<char, char[]>
        //    {
        //        ['a'] = new[] { '4', '@' },
        //        ['b'] = new[] { '8' },
        //        ['c'] = new[] { '(', '{', '[', '<' },
        //        ['e'] = new[] { '3' },
        //        ['g'] = new[] { '6', '9' },
        //        ['i'] = new[] { '1', '!', '|' },
        //        ['l'] = new[] { '1', '|', '7' },
        //        ['o'] = new[] { '0' },
        //        ['s'] = new[] { '$', '5' },
        //        ['t'] = new[] { '+', '7' },
        //        ['x'] = new[] { '%' },
        //        ['z'] = new[] { '2' }
        //    };
        //    return subs;
        //}

        //// ReSharper disable once InconsistentNaming
        //private static void CalulateL33tEntropy(L33tDictionaryMatch match)
        //{
        //    // I'm a bit dubious about this function, but I have duplicated zxcvbn functionality regardless

        //    var possibilities = 0;

        //    foreach (var kvp in match.Subs)
        //    {
        //        var subbedChars = match.Token.Count(c => c == kvp.Key);
        //        var unsubbedChars = match.Token.Count(c => c == kvp.Value); // Won't this always be zero?

        //        possibilities += Enumerable.Range(0, Math.Min(subbedChars, unsubbedChars) + 1).Sum(i => (int)PasswordScoring.Binomial(subbedChars + unsubbedChars, i));
        //    }

        //    var entropy = Math.Log(possibilities, 2);

        //    // In the case of only a single subsitution (e.g. 4pple) this would otherwise come out as zero, so give it one bit
        //    match.L33tEntropy = (entropy < 1 ? 1 : entropy);
        //    match.Entropy += match.L33tEntropy;

        //    // We have to recalculate the uppercase entropy -- the password matcher will have used the subbed password not the original text
        //    match.Entropy -= match.UppercaseEntropy;
        //    match.UppercaseEntropy = PasswordScoring.CalculateUppercaseEntropy(match.Token);
        //    match.Entropy += match.UppercaseEntropy;
        //}

        //private static List<Dictionary<char, char>> EnumerateSubtitutions(Dictionary<char, char[]> table)
        //{
        //    var keys = table.Keys;
        //    var subs = new List<Tuple<char, char>>();

        //    IEnumerable<Tuple<char, char>> Dedup(IEnumerable<Tuple<char, char>> inputSubs)
        //    {
        //        return inputSubs.Distinct(new SubstituionComparer());
        //    }

        //    void Helper(ReadOnlyDictionary<char, char[]> table, IReadOnlyCollection<char> keys, List<Tuple<char, char>> substitutions)
        //    {
        //        if (!keys.Any())
        //            return;

        //        var firstKey = keys.First();
        //        var restKeys = keys.Skip(1);

        //        foreach (var c in table[firstKey])
        //        {
        //            foreach (var sub in substitutions)
        //            {
        //                var duplicateL33tIndex = -1;
        //                for (int i = 0; i < sub.)
        //            }
        //        }
        //    }

        //    // Produce a list of maps from l33t character to normal character. Some substitutions can be more than one normal character though,
        //    //  so we have to produce an entry that maps from the l33t char to both possibilities

        //    //XXX: This function produces different combinations to the original in zxcvbn. It may require some more work to get identical.

        //    //XXX: The function is also limited in that it only ever considers one substitution for each l33t character (e.g. ||ke could feasibly
        //    //     match 'like' but this method would never show this). My understanding is that this is also a limitation in zxcvbn and so I
        //    //     feel no need to correct it here.

        //    var subs = new List<Dictionary<char, char>>
        //    {
        //        new Dictionary<char, char>() // Must be at least one mapping dictionary to work
        //    };
        //    foreach (var mapPair in table)
        //    {
        //        var normalChar = mapPair.Key;

        //        // ReSharper disable once InconsistentNaming
        //        foreach (var l33tChar in mapPair.Value)
        //        {
        //            // Can't add while enumerating so store here
        //            var addedSubs = new List<Dictionary<char, char>>();

        //            foreach (var subDict in subs)
        //            {
        //                if (subDict.ContainsKey(l33tChar))
        //                {
        //                    // This mapping already contains a corresponding normal character for this character, so keep the existing one as is
        //                    //   but add a duplicate with the mappring replaced with this normal character
        //                    var newSub = new Dictionary<char, char>(subDict) { [l33tChar] = normalChar };
        //                    addedSubs.Add(newSub);
        //                }
        //                else
        //                {
        //                    subDict[l33tChar] = normalChar;
        //                }
        //            }

        //            subs.AddRange(addedSubs);
        //        }
        //    }

        //    return subs;
        //}

        //// ReSharper disable once InconsistentNaming
        //private static ReadOnlyDictionary<char, char[]> RelevantL33tSubtable(string password)
        //{
        //    var subtable = new Dictionary<char, char[]>();

        //    foreach (var c in password)
        //    {
        //        if (_l33tTable.ContainsKey(c))
        //            subtable[c] = _l33tTable[c];
        //    }

        //    return new ReadOnlyDictionary<char, char[]>(subtable);
        //}

        //private static string TranslateString(IReadOnlyDictionary<char, char> charMap, string str)
        //{
        //    // Make substitutions from the character map wherever possible
        //    return new string(str.Select(c => charMap.ContainsKey(c) ? charMap[c] : c).ToArray());
        //}

        //private Dictionary<char, char[]> GetRelevantSubstitutions(string password)
        //{
        //    // Return a map of only the useful substitutions, i.e. only characters that the password
        //    //   contains a substituted form of
        //    return _substitutions.Where(kv => kv.Value.Any(password.Contains))
        //                        .ToDictionary(kv => kv.Key, kv => kv.Value.Where(password.Contains).ToArray());
        //}

        //private class SubstituionComparer : IEqualityComparer<Tuple<char, char>>
        //{
        //    public bool Equals(Tuple<char, char> x, Tuple<char, char> y)
        //    {
        //        return x.Item1 == y.Item1 && x.Item2 == y.Item2;
        //    }

        //    public int GetHashCode(Tuple<char, char> obj)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        public IEnumerable<Match> MatchPassword(string password)
        {
            throw new NotImplementedException();
        }
    }
}