using System.Collections.Generic;

namespace Zxcvbn
{
    /// <summary>
    /// The results of zxcvbn's password analysis
    /// </summary>
    // TODO: These should probably be immutable
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