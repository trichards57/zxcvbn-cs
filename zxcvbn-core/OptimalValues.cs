using System.Collections.Generic;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// Represents the optimal value when assessing the most guessible.
    /// </summary>
    internal class OptimalValues
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptimalValues"/> class.
        /// </summary>
        /// <param name="length">The length of the password being assessed.</param>
        public OptimalValues(int length)
        {
            for (var i = 0; i < length; i++)
            {
                G.Add(new Dictionary<int, double>());
                Pi.Add(new Dictionary<int, double>());
                M.Add(new Dictionary<int, Match>());
            }
        }

        /// <summary>
        /// Gets or sets the overall metric for the best guess.
        /// </summary>
        public List<Dictionary<int, double>> G { get; set; } = new List<Dictionary<int, double>>();

        /// <summary>
        /// Gets or sets the best match at a given length.
        /// </summary>
        public List<Dictionary<int, Match>> M { get; set; } = new List<Dictionary<int, Match>>();

        /// <summary>
        /// Gets or sets the the product term of the metric for the best guess.
        /// </summary>
        public List<Dictionary<int, double>> Pi { get; set; } = new List<Dictionary<int, double>>();
    }
}
