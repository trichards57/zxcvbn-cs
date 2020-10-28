using System.Collections.Generic;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// <para>Zxcvbn is used to estimate the strength of passwords. </para>
    ///
    /// <para>This implementation is a port of the Zxcvbn JavaScript library by Dan Wheeler:
    /// https://github.com/lowe/zxcvbn</para>
    ///
    /// <para>To quickly evaluate a password, use the <see cref="MatchPassword"/> static function.</para>
    ///
    /// <para>To evaluate a number of passwords, create an instance of this object and repeatedly call the <see cref="EvaluatePassword"/> function.
    /// Reusing the the Zxcvbn instance will ensure that pattern matchers will only be created once rather than being recreated for each password
    /// e=being evaluated.</para>
    /// </summary>
    public class Zxcvbn
    {
        /// <summary>
        /// <para>Perform the password matching on the given password and user inputs, returing the result structure with information
        /// on the lowest entropy match found.</para>
        ///
        /// <para>User data will be treated as another kind of dictionary matching, but can be different for each password being evaluated.</para>para>
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="userInputs">Optionally, an enumarable of user data</param>
        /// <returns>Result for lowest entropy match</returns>
        public static Result EvaluatePassword(string password, IEnumerable<string> userInputs = null)
        {
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
                Feedback = feedback
            };

            return finalResult;
        }

        public static IEnumerable<Match> GetAllMatches(string token, IEnumerable<string> userInputs = null)
        {
            userInputs = userInputs ?? Enumerable.Empty<string>();

            return new DefaultMatcherFactory().CreateMatchers(userInputs).SelectMany(matcher => matcher.MatchPassword(token));
        }
    }
}
