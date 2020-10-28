using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Attempts to match a string with a list of words.
    /// </summary>
    /// <remarks>
    /// This matcher reads in a list of words (in frequency order) either from a built in resource, an external file
    /// or a list of strings.  These must be in decreasing frequency order and contain one word per line with no additional information.
    /// </remarks>
    internal class DictionaryMatcher : IMatcher
    {
        private readonly string dictionaryName;
        private readonly Dictionary<string, int> rankedDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMatcher"/> class.
        /// </summary>
        /// <param name="name">The name provided to the dictionary used.</param>
        /// <param name="wordListPath">The filename of the dictionary (full or relative path) or name of built-in dictionary.</param>
        public DictionaryMatcher(string name, string wordListPath)
        {
            dictionaryName = name;
            rankedDictionary = BuildRankedDictionary(wordListPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryMatcher"/> class.
        /// Creates a new dictionary matcher from the passed in word list. If there is any frequency order then they should be in
        /// decreasing frequency order.
        /// </summary>
        /// <param name="name">The name provided to the dictionary used.</param>
        /// <param name="wordList">The words to add to the dictionary.</param>
        public DictionaryMatcher(string name, IEnumerable<string> wordList)
        {
            dictionaryName = name;

            // Must ensure that the dictionary is using lowercase words only
            rankedDictionary = BuildRankedDictionary(wordList.Select(w => w.ToLower(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Find dictionary matches in <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>An enumerable of dictionary matches.</returns>
        public virtual IEnumerable<Match> MatchPassword(string password)
        {
            var passwordLower = password.ToLower(CultureInfo.InvariantCulture);
            var length = passwordLower.Length;
            var matches = new List<Match>();

            for (var i = 0; i < length; i++)
            {
                for (var j = i; j < length; j++)
                {
                    var passwordSub = passwordLower.Substring(i, j - i + 1);
                    if (rankedDictionary.ContainsKey(passwordSub))
                    {
                        var match = new DictionaryMatch
                        {
                            i = i,
                            j = j,
                            Token = password.Substring(i, j - i + 1),
                            MatchedWord = passwordSub,
                            Rank = rankedDictionary[passwordSub],
                            DictionaryName = dictionaryName,
                            Reversed = false,
                            L33t = false,
                        };

                        matches.Add(match);
                    }
                }
            }

            return matches.OrderBy(m => m.i).ThenBy(m => m.j);
        }

        /// <summary>
        /// Loads the file <paramref name="wordListFile"/>.
        /// </summary>
        ///
        /// If the file is embedded in the assembly, this is loaded, otherwise <paramref name="wordListFile"/> is treated as a file path.
        /// <param name="wordListFile">Path to word list.</param>
        /// <returns>A dictionary of the words and their associated rank.</returns>
        private static Dictionary<string, int> BuildRankedDictionary(string wordListFile)
        {
            var lines = Utility.GetEmbeddedResourceLines($"Zxcvbn.Dictionaries.{wordListFile}") ?? File.ReadAllLines(wordListFile);

            return BuildRankedDictionary(lines);
        }

        private static Dictionary<string, int> BuildRankedDictionary(IEnumerable<string> wordList)
        {
            var dict = new Dictionary<string, int>();

            var i = 1;
            foreach (var word in wordList)
            {
                var actualWord = word;
                if (actualWord.Contains(" "))
                    actualWord = actualWord.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];

                // The word list is assumed to be in increasing frequency order
                dict[actualWord] = i++;
            }

            return dict;
        }
    }
}
