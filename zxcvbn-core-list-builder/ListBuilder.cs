using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Zxcvbn.ListBuilder
{
    internal static class ListBuilder
    {
        /// <summary>
        /// Returns the maximum number of words for a dictionary.
        /// null means take everything.
        /// </summary>
        private static readonly ReadOnlyDictionary<string, int?> DictionaryLimits = new ReadOnlyDictionary<string, int?>(new Dictionary<string, int?>
        {
            { "us_tv_and_film", 30000 },
            { "english", 30000 },
            { "passwords", 30000 },
            { "surnames", 10000 },
            { "male_names", null },
            { "female_names", null },
        });

        /// <summary>
        /// Filters the frequency lists to remove:
        /// - Tokens which are short and rare
        /// - Tokens that are already in another dictionary and a lower rank
        /// - Tokens that end up beyond the end of the limits in <see cref="DictionaryLimits"/>.
        /// </summary>
        /// <param name="frequencyLists">The list to filter.</param>
        /// <returns>
        /// The filtered output.
        /// </returns>
        public static Dictionary<string, List<string>> FilterFrequencyLists(Dictionary<string, Dictionary<string, int>> frequencyLists)
        {
            var filteredTokenAndRank = new Dictionary<string, Dictionary<string, int>>();
            var tokenCounts = new Dictionary<string, int>();

            foreach (var name in frequencyLists.Keys)
            {
                filteredTokenAndRank[name] = new Dictionary<string, int>();
                tokenCounts[name] = 0;
            }

            var minimumValues = new Dictionary<string, RankDictionaryName>();

            foreach (var kvp in frequencyLists)
            {
                var name = kvp.Key;
                foreach (var tvp in kvp.Value)
                {
                    var token = tvp.Key;
                    var rank = tvp.Value;

                    if (!minimumValues.ContainsKey(token))
                    {
                        minimumValues[token] = new RankDictionaryName
                        {
                            Name = name,
                            Rank = rank,
                        };
                    }
                    else
                    {
                        if (rank < minimumValues[token].Rank)
                        {
                            minimumValues[token] = new RankDictionaryName
                            {
                                Rank = rank,
                                Name = name,
                            };
                        }
                    }
                }
            }

            foreach (var kvp in frequencyLists)
            {
                var name = kvp.Key;
                foreach (var tvp in kvp.Value)
                {
                    var token = tvp.Key;
                    var rank = tvp.Value;

                    if (token == "o")
                    {
                        Console.Write(string.Empty);
                    }

                    if (minimumValues[token].Name != name)
                        continue;
                    if (IsRareAndShort(token, rank) || HasCommaOrDoubleQuote(token))
                        continue;
                    filteredTokenAndRank[name][token] = rank;
                    tokenCounts[name]++;
                }
            }

            var result = new Dictionary<string, List<string>>();

            foreach (var kvp in filteredTokenAndRank)
            {
                var name = kvp.Key;
                var values = kvp.Value;
                var res = values.OrderBy(s => s.Value).Select(kvp => kvp.Key);
                if (DictionaryLimits[name].HasValue)
                    res = res.Take(DictionaryLimits[name].Value);
                result[name] = res.ToList();
            }

            return result;
        }

        public static Dictionary<string, Dictionary<string, int>> ParseFrequencyLists(string directory)
        {
            var result = new Dictionary<string, Dictionary<string, int>>();

            foreach (var file in Directory.GetFiles(directory))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!DictionaryLimits.ContainsKey(name))
                {
                    Console.WriteLine($"Warning: {name} is in directory {directory} but was not expected.  Skipped.");
                    continue;
                }

                var tokenToRank = new Dictionary<string, int>();
                var rank = 0;

                foreach (var line in File.ReadAllLines(file).Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    rank++;
                    var token = line.Split(" ")[0];
                    tokenToRank[token] = rank;
                }

                result[name] = tokenToRank;
            }

            foreach (var expectedKey in DictionaryLimits.Keys)
            {
                if (!result.ContainsKey(expectedKey))
                {
                    Console.WriteLine($"Warning: {expectedKey} is not in directory {directory} but was expected.");
                }
            }

            return result;
        }

        private static bool HasCommaOrDoubleQuote(string token)
        {
            return token.Contains(",") || token.Contains("\"");
        }

        private static bool IsRareAndShort(string token, int rank)
        {
            return rank >= Math.Pow(10, token.Length);
        }
    }
}
