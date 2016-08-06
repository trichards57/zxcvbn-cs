using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Matches found by the dictionary matcher contain some additional information about the matched word.
    /// </summary>
    public class DictionaryMatch : Match
    {
        /// <summary>
        /// The base entropy of the match, calculated from frequency rank
        /// </summary>
        public double BaseEntropy { get; set; }

        /// <summary>
        /// The name of the dictionary the matched word was found in
        /// </summary>
        public string DictionaryName { get; set; }

        /// <summary>
        /// The dictionary word matched
        /// </summary>
        public string MatchedWord { get; set; }

        /// <summary>
        /// The rank of the matched word in the dictionary (i.e. 1 is most frequent, and larger numbers are less common words)
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Additional entropy for this match from the use of mixed case
        /// </summary>
        public double UppercaseEntropy { get; set; }
    }

    /// <summary>
    /// <para>This matcher reads in a list of words (in frequency order) and matches substrings of the password against that dictionary.</para>
    ///
    /// <para>The dictionary to be used can be specified directly by passing an enumerable of strings through the constructor (e.g. for
    /// matching agains user inputs). Most dictionaries will be in word list files.</para>
    ///
    /// <para>Using external files is a departure from the JS version of Zxcvbn which bakes in the word lists, so the default dictionaries
    /// have been included in the Zxcvbn assembly as embedded resources (to remove the external dependency). Thus when a word list is specified
    /// by name, it is first checked to see if it matches and embedded resource and if not is assumed to be an external file. </para>
    ///
    /// <para>Thus custom dictionaries can be included by providing the name of an external text file, but the built-in dictionaries (english.lst,
    /// female_names.lst, male_names.lst, passwords.lst, surnames.lst) can be used without concern about locating a dictionary file in an accessible
    /// place.</para>
    ///
    /// <para>Dictionary word lists must be in decreasing frequency order and contain one word per line with no additional information.</para>
    /// </summary>
    public class DictionaryMatcher : IMatcher
    {
        private const string DictionaryPattern = "dictionary";

        private string dictionaryName;
        private Lazy<Dictionary<string, int>> rankedDictionary;

        /// <summary>
        /// Creates a new dictionary matcher. <paramref name="wordListPath"/> must be the path (relative or absolute) to a file containing one word per line,
        /// entirely in lowercase, ordered by frequency (decreasing); or <paramref name="wordListPath"/> must be the name of a built-in dictionary.
        /// </summary>
        /// <param name="name">The name provided to the dictionary used</param>
        /// <param name="wordListPath">The filename of the dictionary (full or relative path) or name of built-in dictionary</param>
        public DictionaryMatcher(string name, string wordListPath)
        {
            this.dictionaryName = name;
            rankedDictionary = new Lazy<Dictionary<string, int>>(() => BuildRankedDictionary(wordListPath));
        }

        /// <summary>
        /// Creates a new dictionary matcher from the passed in word list. If there is any frequency order then they should be in
        /// decreasing frequency order.
        /// </summary>
        public DictionaryMatcher(string name, IEnumerable<string> wordList)
        {
            this.dictionaryName = name;

            // Must ensure that the dictionary is using lowercase words only
            rankedDictionary = new Lazy<Dictionary<string, int>>(() => BuildRankedDictionary(wordList.Select(w => w.ToLower())));
        }

        /// <summary>
        /// Match substrings of password agains the loaded dictionary
        /// </summary>
        /// <param name="password">The password to match</param>
        /// <returns>An enumerable of dictionary matches</returns>
        /// <seealso cref="DictionaryMatch"/>
        public IEnumerable<Match> MatchPassword(string password)
        {
            var passwordLower = password.ToLower();

            var matches = (from i in Enumerable.Range(0, password.Length)
                           from j in Enumerable.Range(i, password.Length - i)
                           let psub = passwordLower.Substring(i, j - i + 1)
                           where rankedDictionary.Value.ContainsKey(psub)
                           select new DictionaryMatch()
                           {
                               Pattern = DictionaryPattern,
                               i = i,
                               j = j,
                               Token = password.Substring(i, j - i + 1), // Could have different case so pull from password
                               MatchedWord = psub,
                               Rank = rankedDictionary.Value[psub],
                               DictionaryName = dictionaryName,
                               Cardinality = rankedDictionary.Value.Count
                           }).ToList();

            foreach (var match in matches) CalculateEntropyForMatch(match);

            return matches;
        }

        private Dictionary<string, int> BuildRankedDictionary(string wordListFile)
        {
            // Look first to wordlists embedded in assembly (i.e. default dictionaries) otherwise treat as file path

            var lines = Utility.GetEmbeddedResourceLines("zxcvbn-core.Dictionaries.{0}".F(wordListFile)) ?? File.ReadAllLines(wordListFile);

            return BuildRankedDictionary(lines);
        }

        private Dictionary<string, int> BuildRankedDictionary(IEnumerable<string> wordList)
        {
            var dict = new Dictionary<string, int>();

            var i = 1;
            foreach (var word in wordList)
            {
                // The word list is assumed to be in increasing frequency order
                dict[word] = i++;
            }

            return dict;
        }

        private void CalculateEntropyForMatch(DictionaryMatch match)
        {
            match.BaseEntropy = Math.Log(match.Rank, 2);
            match.UppercaseEntropy = PasswordScoring.CalculateUppercaseEntropy(match.Token);

            match.Entropy = match.BaseEntropy + match.UppercaseEntropy;
        }
    }
}