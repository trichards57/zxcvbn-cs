using System;

namespace Zxcvbn
{
    /// <summary>
    /// <para>A single match that one of the pattern matchers has made against the password being tested.</para>
    ///
    /// <para>Some pattern matchers implement subclasses of match that can provide more information on their specific results.</para>
    ///
    /// <para>Matches must all have the <see cref="Pattern"/>, <see cref="Token"/>, <see cref="Entropy"/>, <see cref="i"/> and
    /// <see cref="j"/> fields (i.e. all but the <see cref="Cardinality"/> field, which is optional) set before being returned from the matcher
    /// in which they are created.</para>
    /// </summary>
    // TODO: These should probably be immutable
    public class Match
    {
        public double Guesses { get; set; }
        public double GuessesLog10 => Math.Log10(Guesses);

        /// <summary>
        /// The start index in the password string of the matched token.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        public int i { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The end index in the password string of the matched token.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        public int j { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        /// <summary>
        /// The name of the pattern matcher used to generate this match
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The portion of the password that was matched
        /// </summary>
        public string Token { get; set; }
    }
}
