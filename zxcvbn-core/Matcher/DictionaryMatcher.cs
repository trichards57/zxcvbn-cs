using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    ///  <inheritdoc />
    ///  <summary>
    ///  <para>This matcher reads in a list of words (in frequency order) and matches substrings of the password against that dictionary.</para>
    ///  <para>The dictionary to be used can be specified directly by passing an enumerable of strings through the constructor (e.g. for
    ///  matching agains user inputs). Most dictionaries will be in word list files.</para>
    ///  <para>Using external files is a departure from the JS version of Zxcvbn which bakes in the word lists, so the default dictionaries
    ///  have been included in the Zxcvbn assembly as embedded resources (to remove the external dependency). Thus when a word list is specified
    ///  by name, it is first checked to see if it matches and embedded resource and if not is assumed to be an external file. </para>
    ///  <para>Thus custom dictionaries can be included by providing the name of an external text file, but the built-in dictionaries (english.lst,
    ///  female_names.lst, male_names.lst, passwords.lst, us_tv_and_film.lst, surnames.lst) can be used without concern about locating a dictionary file in an accessible
    ///  place.</para>
    ///  <para>Dictionary word lists must be in decreasing frequency order and contain one word per line with no additional information.</para>
    ///  </summary>
    public class DictionaryMatcher : IMatcher
    {
        public const string DictionaryPattern = "dictionary";

        private readonly string _dictionaryName;
        private readonly Dictionary<string, int> _rankedDictionary;

        /// <summary>
        /// Creates a new dictionary matcher. <paramref name="wordListPath"/> must be the path (relative or absolute) to a file containing one word per line,
        /// entirely in lowercase, ordered by frequency (decreasing); or <paramref name="wordListPath"/> must be the name of a built-in dictionary.
        /// </summary>
        /// <param name="name">The name provided to the dictionary used</param>
        /// <param name="wordListPath">The filename of the dictionary (full or relative path) or name of built-in dictionary</param>
        public DictionaryMatcher(string name, string wordListPath)
        {
            _dictionaryName = name;
            _rankedDictionary = BuildRankedDictionary(wordListPath);
        }

        /// <summary>
        /// Creates a new dictionary matcher from the passed in word list. If there is any frequency order then they should be in
        /// decreasing frequency order.
        /// </summary>
        public DictionaryMatcher(string name, IEnumerable<string> wordList)
        {
            _dictionaryName = name;

            // Must ensure that the dictionary is using lowercase words only
            _rankedDictionary = BuildRankedDictionary(wordList.Select(w => w.ToLower()));
        }

        /// <inheritdoc />
        /// <summary>
        /// Match substrings of password agains the loaded dictionary
        /// </summary>
        /// <param name="password">The password to match</param>
        /// <returns>An enumerable of dictionary matches</returns>
        /// <seealso cref="T:Zxcvbn.Matcher.DictionaryMatch" />
        public virtual IEnumerable<Match> MatchPassword(string password)
        {
            var passwordLower = password.ToLower();
            var length = passwordLower.Length;
            var matches = new List<Match>();

            for (var i = 0; i < length; i++)
            {
                for (var j = i; j < length; j++)
                {
                    var passwordSub = passwordLower.Substring(i, j - i + 1);
                    if (_rankedDictionary.ContainsKey(passwordSub))
                    {
                        var match = new DictionaryMatch
                        {
                            i = i,
                            j = j,
                            Token = password.Substring(i, j - i + 1),
                            MatchedWord = passwordSub,
                            Rank = _rankedDictionary[passwordSub],
                            DictionaryName = _dictionaryName,
                            Reversed = false,
                            L33t = false
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
