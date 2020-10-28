using System.Collections.Generic;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// The results of zxcvbn's password analysis.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets the number of milliseconds that zxcvbn took to calculate results for this password.
        /// </summary>
        public long CalcTime { get; internal set; }

        /// <summary>
        /// Gets An estimation of the crack times for this password in seconds.
        /// </summary>
        public CrackTimes CrackTime { get; internal set; }

        /// <summary>
        /// Gets a set of string for the crack times.
        /// </summary>
        public CrackTimesDisplay CrackTimeDisplay { get; internal set; }

        /// <summary>
        /// Gets the feedback for the user about their password.
        /// </summary>
        public FeedbackItem Feedback { get; internal set; }

        /// <summary>
        /// Gets the number of guesses the password is estimated to need.
        /// </summary>
        public double Guesses { get; internal set; }

        /// <summary>
        /// Gets the sequence of matches that were used to assess the password.
        /// </summary>
        public IEnumerable<Match> MatchSequence { get; internal set; }

        /// <summary>
        /// Gets the password that was used to generate these results.
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        /// Gets a score from 0 to 4 (inclusive), with 0 being least secure and 4 being most secure, calculated from the nubmer of guesses estimated to be needed.
        /// </summary>
        public int Score { get; internal set; }
    }
}
