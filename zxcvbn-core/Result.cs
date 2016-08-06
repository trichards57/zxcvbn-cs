using System.Collections.Generic;

// TODO: These should probably be immutable
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
    public class Match
    {
        /// <summary>
        /// Some pattern matchers can associate the cardinality of the set of possible matches that the
        /// entropy calculation is derived from. Not all matchers provide a value for cardinality.
        /// </summary>
        public int Cardinality { get; set; }

        /// <summary>
        /// The entropy that this portion of the password covers using the current pattern matching technique
        /// </summary>
        public double Entropy { get; set; }

        // The following are more internal measures, but may be useful to consumers
        /// <summary>
        /// The start index in the password string of the matched token.
        /// </summary>
        public int i { get; set; }

        /// <summary>
        /// The end index in the password string of the matched token.
        /// </summary>
        public int j { get; set; }

        /// <summary>
        /// The name of the pattern matcher used to generate this match
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The portion of the password that was matched
        /// </summary>
        public string Token { get; set; }

        // Start Index

        // End Index
    }

    /// <summary>
    /// The results of zxcvbn's password analysis
    /// </summary>
    public class Result
    {
        /// <summary>
        /// The number of milliseconds that zxcvbn took to calculate results for this password
        /// </summary>
        public long CalcTime { get; set; }

        /// <summary>
        /// An estimation of the crack time for this password in seconds
        /// </summary>
        public double CrackTime { get; set; }

        /// <summary>
        /// A friendly string for the crack time (like "centuries", "instant", "7 minutes", "14 hours" etc.)
        /// </summary>
        public string CrackTimeDisplay { get; set; }

        /// <summary>
        /// A calculated estimate of how many bits of entropy the password covers, rounded to three decimal places.
        /// </summary>
        public double Entropy { get; set; }

        /// <summary>
        /// The sequence of matches that were used to create the entropy calculation
        /// </summary>
        public IList<Match> MatchSequence { get; set; }

        /// <summary>
        /// The password that was used to generate these results
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// A score from 0 to 4 (inclusive), with 0 being least secure and 4 being most secure calculated from crack time:
        /// [0,1,2,3,4] if crack time is less than [10**2, 10**4, 10**6, 10**8, Infinity] seconds.
        /// Useful for implementing a strength meter
        /// </summary>
        public int Score { get; set; }
    }
}