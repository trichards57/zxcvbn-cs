﻿using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    public class RepeatGuessesCalculator
    {
        public double CalculateGuesses(RepeatMatch match)
        {
            return match.BaseGuesses * match.RepeatCount;
        }
    }
}