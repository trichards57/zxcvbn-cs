using System.Collections.Generic;

namespace Zxcvbn.Matcher.Matches
{
    /// <inheritdoc />
    /// <summary>
    /// L33tMatcher results are like dictionary match results with some extra information that pertains to the extra entropy that
    /// is garnered by using substitutions.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class L33tDictionaryMatch : DictionaryMatch
    {
        /// <summary>
        /// Create a new l33t match from a dictionary match
        /// </summary>
        /// <param name="dm">The dictionary match to initialise the l33t match from</param>
        public L33tDictionaryMatch(DictionaryMatch dm)
        {
            BaseEntropy = dm.BaseEntropy;
            DictionaryName = dm.DictionaryName;
            i = dm.i;
            j = dm.j;
            MatchedWord = dm.MatchedWord;
            Pattern = dm.Pattern;
            Rank = dm.Rank;
            Token = dm.Token;
            UppercaseEntropy = dm.UppercaseEntropy;
        }

        public L33tDictionaryMatch()
        {
        }

        /// <summary>
        /// The character mappings that are in use for this match
        /// </summary>
        public Dictionary<char, char> Sub { get; set; }
    }
}
