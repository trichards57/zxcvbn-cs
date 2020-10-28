using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    internal class RepeatGuessesCalculator
    {
        public static double CalculateGuesses(RepeatMatch match)
        {
            return match.BaseGuesses * match.RepeatCount;
        }
    }
}
