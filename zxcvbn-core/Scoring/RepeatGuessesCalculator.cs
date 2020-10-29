using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to guesses the number of repeats.
    /// </summary>
    internal class RepeatGuessesCalculator
    {
        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static double CalculateGuesses(RepeatMatch match)
        {
            return match.BaseGuesses * match.RepeatCount;
        }
    }
}
