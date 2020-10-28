using System.Collections.Generic;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// Represents a class that can look for matches in a password.
    /// </summary>
    internal interface IMatcher
    {
        /// <summary>
        /// Find matches in <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>An enumerable of the matches.</returns>
        IEnumerable<Match> MatchPassword(string password);
    }
}
