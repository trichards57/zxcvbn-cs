using System.Collections.Generic;

namespace Zxcvbn.Matcher
{
    /// <summary>
    /// All pattern matchers must implement the IMatcher interface.
    /// </summary>
    public interface IMatcher
    {
        /// <summary>
        /// This function is called once for each matcher for each password being evaluated. It should perform the matching process and return
        /// an enumerable of Match objects for each match found.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        IEnumerable<Match> MatchPassword(string password);
    }
}