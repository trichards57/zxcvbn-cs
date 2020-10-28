using System.Collections.Generic;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// Estimates the strength of passwords.
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Perform the password matching on the given password and user inputs.
        /// </summary>
        /// <param name="password">The password to assess.</param>
        /// <param name="userInputs">Optionally, an enumarable of user data.</param>
        /// <returns>Result for the password.</returns>
        public static Result EvaluatePassword(string password, IEnumerable<string> userInputs = null)
        {
            password = password ?? string.Empty;
            userInputs = userInputs ?? Enumerable.Empty<string>();

            var timer = System.Diagnostics.Stopwatch.StartNew();

            var matches = GetAllMatches(password, userInputs);
            var result = PasswordScoring.MostGuessableMatchSequence(password, matches);
            timer.Stop();

            var attackTimes = TimeEstimates.EstimateAttackTimes(result.Guesses);
            var feedback = Feedback.GetFeedback(result.Score, result.Sequence);

            var finalResult = new Result
            {
                CalcTime = timer.ElapsedMilliseconds,
                CrackTime = attackTimes.CrackTimesSeconds,
                CrackTimeDisplay = attackTimes.CrackTimesDisplay,
                Score = attackTimes.Score,
                MatchSequence = result.Sequence,
                Guesses = result.Guesses,
                Password = result.Password,
                Feedback = feedback,
            };

            return finalResult;
        }

        /// <summary>
        /// Gets all matches associated with a password.
        /// </summary>
        /// <param name="token">The password to assess.</param>
        /// <param name="userInputs">The user input dictionary.</param>
        /// <returns>An enumerable of relevant matches.</returns>
        internal static IEnumerable<Match> GetAllMatches(string token, IEnumerable<string> userInputs = null)
        {
            userInputs = userInputs ?? Enumerable.Empty<string>();

            return DefaultMatcherFactory.CreateMatchers(userInputs).SelectMany(matcher => matcher.MatchPassword(token));
        }
    }
}
