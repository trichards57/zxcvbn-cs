﻿using System;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    public class RegexGuessesCalculator
    {
        private const int MinimumYearSpace = 20;

        public double CalculateGuesses(RegexMatch match)
        {
            switch (match.RegexName)
            {
                case "recent_year":
                    var yearSpace = Math.Abs(int.Parse(match.Pattern) - DateMatcher.ReferenceYear);
                    yearSpace = Math.Max(yearSpace, MinimumYearSpace);
                    return yearSpace;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}