using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    // Instances of Point or Pair in the standard library are in UI assemblies, so define our own version to reduce dependencies
    internal struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override string ToString()
        {
            return "{" + X + ", " + Y + "}";
        }
    }

    // See build_keyboard_adjacency_graph.py in zxcvbn for how these are generated
    internal class SpatialGraph
    {
        public SpatialGraph(string name, string layout, bool slanted)
        {
            Name = name;
            BuildGraph(layout, slanted);
        }

        public Dictionary<char, List<string>> AdjacencyGraph { get; private set; }
        public string Name { get; }

        private static Point[] GetAlignedAdjacent(Point c)
        {
            var x = c.X;
            var y = c.Y;

            return new[] { new Point(x - 1, y), new Point(x - 1, y - 1), new Point(x, y - 1), new Point(x + 1, y - 1), new Point(x + 1, y), new Point(x + 1, y + 1), new Point(x, y + 1), new Point(x - 1, y + 1) };
        }

        private static Point[] GetSlantedAdjacent(Point c)
        {
            var x = c.X;
            var y = c.Y;

            return new[] { new Point(x - 1, y), new Point(x, y - 1), new Point(x + 1, y - 1), new Point(x + 1, y), new Point(x, y + 1), new Point(x - 1, y + 1) };
        }

        private void BuildGraph(string layout, bool slanted)
        {
            var tokens = layout.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var tokenSize = tokens[0].Length;

            // Put the characters in each keyboard cell into the map agains t their coordinates
            var positionTable = new Dictionary<Point, string>();
            var lines = layout.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (var y = 0; y < lines.Length; ++y)
            {
                var line = lines[y];
                var slant = slanted ? y - 1 : 0;

                foreach (var token in line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                {
                    var x = (line.IndexOf(token, StringComparison.Ordinal) - slant) / (tokenSize + 1);
                    var p = new Point(x, y);
                    positionTable[p] = token;
                }
            }

            AdjacencyGraph = new Dictionary<char, List<string>>();
            foreach (var pair in positionTable)
            {
                var p = pair.Key;
                foreach (var c in pair.Value)
                {
                    AdjacencyGraph[c] = new List<string>();
                    var adjacentPoints = slanted ? GetSlantedAdjacent(p) : GetAlignedAdjacent(p);

                    foreach (var adjacent in adjacentPoints)
                    {
                        // We want to include nulls so that direction is correspondent with index in the list
                        AdjacencyGraph[c].Add(positionTable.ContainsKey(adjacent) ? positionTable[adjacent] : null);
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// <para>A matcher that checks for keyboard layout patterns (e.g. 78523 on a keypad, or plkmn on a QWERTY keyboard).</para>
    /// <para>Has patterns for QWERTY, DVORAK, numeric keybad and mac numeric keypad</para>
    /// <para>The matcher accounts for shifted characters (e.g. qwErt or po9*7y) when detecting patterns as well as multiple changes in direction.</para>
    /// </summary>
    internal class SpatialMatcher : IMatcher
    {
        private const string ShiftedRegex = "[~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?]";
        private const string SpatialPattern = "spatial";

        internal List<SpatialGraph> SpatialGraphs { get; set; } = GenerateSpatialGraphs();

        /// <inheritdoc />
        /// <summary>
        /// Match the password against the known keyboard layouts
        /// </summary>
        /// <param name="password">Password to match</param>
        /// <returns>List of matching patterns</returns>
        /// <seealso cref="M:Zxcvbn.Matcher.SpatialMatcher.SpatialMatch(Zxcvbn.Matcher.SpatialMatcher.SpatialGraph,System.String)" />
        public IEnumerable<Matches.Match> MatchPassword(string password)
        {
            return SpatialGraphs.SelectMany(g => SpatialMatch(g, password)).ToList();
        }

        // In the JS version these are precomputed, but for now we'll generate them here when they are first needed.
        private static List<SpatialGraph> GenerateSpatialGraphs()
        {
            // Kwyboard layouts directly from zxcvbn's build_keyboard_adjacency_graph.py script
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
                    new SpatialGraph("mac_keypad", macKeypad, false)
                };
        }

        /// <summary>
        /// Match the password against a single pattern
        /// </summary>
        /// <param name="graph">Adjacency graph for this key layout</param>
        /// <param name="password">Password to match</param>
        /// <returns>List of matching patterns</returns>
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
                                Pattern = SpatialPattern,
                                i = i,
                                j = j - 1,
                                Token = password.Substring(i, j - i),
                                Graph = graph.Name,
                                Turns = turns,
                                ShiftedCount = shiftedCount
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
