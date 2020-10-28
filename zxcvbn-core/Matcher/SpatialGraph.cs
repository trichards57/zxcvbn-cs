using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// A graph detailing how keys are arranged on a typical keyboard.
    /// </summary>
    internal class SpatialGraph
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialGraph"/> class with the given name
        /// and based on the given layout.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="slanted">if set to <c>true</c> the keys are slanted.</param>
        public SpatialGraph(string name, string layout, bool slanted)
        {
            Name = name;
            BuildGraph(layout, slanted);
        }

        /// <summary>
        /// Gets the generated adjacency graph.
        /// </summary>
        public ReadOnlyDictionary<char, ReadOnlyCollection<string>> AdjacencyGraph { get; private set; }

        /// <summary>
        /// Gets the name of the generated graph.
        /// </summary>
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

            var adjacencyGraph = new Dictionary<char, List<string>>();
            foreach (var pair in positionTable)
            {
                var p = pair.Key;
                foreach (var c in pair.Value)
                {
                    adjacencyGraph[c] = new List<string>();
                    var adjacentPoints = slanted ? GetSlantedAdjacent(p) : GetAlignedAdjacent(p);

                    foreach (var adjacent in adjacentPoints)
                    {
                        // We want to include nulls so that direction is correspondent with index in the list
                        adjacencyGraph[c].Add(positionTable.ContainsKey(adjacent) ? positionTable[adjacent] : null);
                    }
                }
            }

            AdjacencyGraph = new ReadOnlyDictionary<char, ReadOnlyCollection<string>>(adjacencyGraph.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AsReadOnly()));
        }
    }
}
