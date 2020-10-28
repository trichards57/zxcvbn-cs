using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Attempts to match a string against common keyboard layout patterns, considering shifted characters and changes in direction.
    /// </summary>
    internal class SpatialMatcher : IMatcher
    {
        private const string ShiftedRegex = "[~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?]";

        /// <summary>
        /// Gets the spatial graphs to match against.
        /// </summary>
        internal static ReadOnlyCollection<SpatialGraph> SpatialGraphs { get; set; } = GenerateSpatialGraphs();

        /// <summary>
        /// Find spatial matches in <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>An enumerable of spatial matches.</returns>
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            return SpatialGraphs.SelectMany(g => SpatialMatch(g, password));
        }

        private static ReadOnlyCollection<SpatialGraph> GenerateSpatialGraphs()
        {
            const string qwerty = @"
`~ 1! 2@ 3# 4$ 5% 6^ 7& 8* 9( 0) -_ =+
    qQ wW eE rR tT yY uU iI oO pP [{ ]} \|
     aA sS dD fF gG hH jJ kK lL ;: '""
      zZ xX cC vV bB nN mM ,< .> /?
";

            const string dvorak = @"
`~ 1! 2@ 3# 4$ 5% 6^ 7& 8* 9( 0) [{ ]}
    '"" ,< .> pP yY fF gG cC rR lL /? =+ \|
     aA oO eE uU iI dD hH tT nN sS -_
      ;: qQ jJ kK xX bB mM wW vV zZ
";

            const string keypad = @"
  / * -
7 8 9 +
4 5 6
1 2 3
  0 .
";

            const string macKeypad = @"
  = / *
7 8 9 -
4 5 6 +
1 2 3
  0 .
";

            return new List<SpatialGraph> { new SpatialGraph("qwerty", qwerty, true),
                    new SpatialGraph("dvorak", dvorak, true),
                    new SpatialGraph("keypad", keypad, false),
                    new SpatialGraph("mac_keypad", macKeypad, false),
                }.AsReadOnly();
        }

        private static IEnumerable<Matches.Match> SpatialMatch(SpatialGraph graph, string password)
        {
            var matches = new List<Matches.Match>();

            var i = 0;
            while (i < password.Length - 1)
            {
                var turns = 0;
                var shiftedCount = 0;
                int? lastDirection = null;

                var j = i + 1;

                if ((graph.Name == "qwerty" || graph.Name == "dvorak") && Regex.IsMatch(password[i].ToString(), ShiftedRegex))
                {
                    shiftedCount = 1;
                }

                while (true)
                {
                    var prevChar = password[j - 1];
                    var found = false;
                    var currentDirection = -1;
                    var adjacents = graph.AdjacencyGraph.ContainsKey(prevChar) ? graph.AdjacencyGraph[prevChar] : Enumerable.Empty<string>();

                    if (j < password.Length)
                    {
                        var curChar = password[j].ToString();
                        foreach (var adjacent in adjacents)
                        {
                            currentDirection++;

                            if (adjacent == null)
                                continue;

                            if (adjacent.Contains(curChar))
                            {
                                found = true;
                                var foundDirection = currentDirection;
                                if (adjacent.IndexOf(curChar, StringComparison.Ordinal) == 1)
                                {
                                    shiftedCount++;
                                }

                                if (lastDirection != foundDirection)
                                {
                                    turns++;
                                    lastDirection = foundDirection;
                                }

                                break;
                            }
                        }
                    }

                    if (found)
                    {
                        j++;
                    }
                    else
                    {
                        if (j - i > 2)
                        {
                            matches.Add(new SpatialMatch()
                            {
                                i = i,
                                j = j - 1,
                                Token = password.Substring(i, j - i),
                                Graph = graph.Name,
                                Turns = turns,
                                ShiftedCount = shiftedCount,
                            });
                        }

                        i = j;
                        break;
                    }
                }
            }

            return matches;
        }
    }
}
