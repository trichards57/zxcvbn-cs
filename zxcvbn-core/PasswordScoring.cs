﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zxcvbn.Matcher.Matches;
using Zxcvbn.Scoring;

namespace Zxcvbn
{
    /// <summary>
    /// Some useful shared functions used for evaluating passwords
    /// </summary>
    internal static class PasswordScoring
    {
        public const string AllUpper = "^[^a-z]+$";
        public const string StartUpper = "^[A-Z][^A-Z]+$";
        private const int MinimumGuessesBeforeGrowingSequence = 10000;

        /// <summary>
        /// Caclulate binomial coefficient (i.e. nCk)
        /// Uses same algorithm as zxcvbn (cf. scoring.coffee), from http://blog.plover.com/math/choose.html
        /// </summary>
        /// <param name="k">k</param>
        /// <param name="n">n</param>
        /// <returns>Binomial coefficient; nCk</returns>
        public static long Binomial(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0) return 1;

            long r = 1;
            for (var d = 1; d <= k; ++d)
            {
                r *= n;
                r /= d;
                n -= 1;
            }

            return r;
        }

        /// <summary>
        /// Return a score for password strength from the crack time. Scores are 0..4, 0 being minimum and 4 maximum strength.
        /// </summary>
        /// <param name="crackTimeSeconds">Number of seconds estimated for password cracking</param>
        /// <returns>Password strength. 0 to 4, 0 is minimum</returns>
        public static int CrackTimeToScore(double crackTimeSeconds)
        {
            if (crackTimeSeconds < Math.Pow(10, 2)) return 0;
            if (crackTimeSeconds < Math.Pow(10, 4)) return 1;
            if (crackTimeSeconds < Math.Pow(10, 6)) return 2;
            if (crackTimeSeconds < Math.Pow(10, 8)) return 3;
            return 4;
        }

        /// <summary>
        /// Calculate a rough estimate of crack time for entropy, see zxcbn scoring.coffee for more information on the model used
        /// </summary>
        /// <param name="entropy">Entropy of password</param>
        /// <returns>An estimation of seconts taken to crack password</returns>
        public static double EntropyToCrackTime(double entropy)
        {
            const double singleGuess = 0.01;
            const double numAttackers = 100;
            const double secondsPerGuess = singleGuess / numAttackers;

            return 0.5 * Math.Pow(2, entropy) * secondsPerGuess;
        }

        public static double EstimateGuesses(Match match, string password)
        {
            if (match.Guesses != 0)
                return match.Guesses;

            var minGuesses = 1;
            if (match.Token.Length < password.Length)
            {
                minGuesses = match.Token.Length == 1 ? BruteForceGuessesCalculator.MinSubmatchGuessesSingleCharacter : BruteForceGuessesCalculator.MinSubmatchGuessesMultiCharacter;
            }

            double guesses = 0;

            switch (match.Pattern)
            {
                case "bruteforce":
                    guesses = BruteForceGuessesCalculator.CalculateGuesses(match as BruteForceMatch);
                    break;

                case "date":
                    guesses = DateGuessesCalculator.CalculateGuesses(match as DateMatch);
                    break;

                case "dictionary":
                    guesses = DictionaryGuessesCalculator.CalculateGuesses(match as DictionaryMatch);
                    break;

                case "regex":
                    guesses = RegexGuessesCalculator.CalculateGuesses(match as RegexMatch);
                    break;

                case "repeat":
                    guesses = RepeatGuessesCalculator.CalculateGuesses(match as RepeatMatch);
                    break;

                case "sequence":
                    guesses = SequenceGuessesCalculator.CalculateGuesses(match as SequenceMatch);
                    break;

                case "spatial":
                    guesses = SpatialGuessesCalculator.CalculateGuesses(match as SpatialMatch);
                    break;
            }

            match.Guesses = Math.Max(guesses, minGuesses);
            return match.Guesses;
        }

        public static MostGuessableMatchResult MostGuessableMatchSequence(string password, IEnumerable<Match> matches, bool excludeAdditive = false)
        {
            var matchesByJ = Enumerable.Range(0, password.Length).Select(i => new List<Match>()).ToList();
            foreach (var m in matches)
                matchesByJ[m.j].Add(m);

            var optimal = new OptimalValues(password.Length);

            for (var k = 0; k < password.Length; k++)
            {
                foreach (var m in matchesByJ[k])
                {
                    if (m.i > 0)
                    {
                        foreach (var l in optimal.M[m.i - 1].Keys)
                        {
                            Update(password, optimal, m, l + 1, excludeAdditive);
                        }
                    }
                    else
                    {
                        Update(password, optimal, m, 1, excludeAdditive);
                    }
                }
                BruteforceUpdate(password, optimal, k, excludeAdditive);
            }

            var optimalMatchSequence = Unwind(optimal, password.Length);
            var optimalL = optimalMatchSequence.Count;

            double guesses;

            if (password.Length == 0)
                guesses = 1;
            else
                guesses = optimal.G[password.Length - 1][optimalL];

            return new MostGuessableMatchResult
            {
                Guesses = guesses,
                Password = password,
                Sequence = optimalMatchSequence,
                Score = 0
            };
        }

        /// <summary>
        /// Calculate the cardinality of the minimal character sets necessary to brute force the password (roughly)
        /// (e.g. lowercase = 26, numbers = 10, lowercase + numbers = 36)
        /// </summary>
        /// <param name="password">THe password to evaluate</param>
        /// <returns>An estimation of the cardinality of charactes for this password</returns>
        public static int PasswordCardinality(string password)
        {
            var cl = 0;

            if (password.Any(c => 'a' <= c && c <= 'z')) cl += 26; // Lowercase
            if (password.Any(c => 'A' <= c && c <= 'Z')) cl += 26; // Uppercase
            if (password.Any(c => '0' <= c && c <= '9')) cl += 10; // Numbers
            if (password.Any(c => c <= '/' || (':' <= c && c <= '@') || ('[' <= c && c <= '`') || ('{' <= c && c <= 0x7F))) cl += 33; // Symbols
            if (password.Any(c => c > 0x7F)) cl += 100; // 'Unicode' (why 100?)

            return cl;
        }

        private static void BruteforceUpdate(string password, OptimalValues optimal, int k, bool excludeAdditive)
        {
            Update(password, optimal, MakeBruteforceMatch(password, 0, k), 1, excludeAdditive);

            for (var i = 1; i <= k; i++)
            {
                var m = MakeBruteforceMatch(password, i, k);
                var obj = optimal.M[i - 1];

                foreach (var l in obj.Keys)
                {
                    var lastM = obj[l];
                    if (lastM.Pattern == "bruteforce")
                        continue;
                    Update(password, optimal, m, l + 1, excludeAdditive);
                }
            }
        }

        private static double Factorial(double n)
        {
            if (n < 2)
                return 1;
            var f = 1;

            for (var i = 2; i <= n; i++)
                f *= i;

            return f;
        }

        private static BruteForceMatch MakeBruteforceMatch(string password, int i, int j)
        {
            return new BruteForceMatch
            {
                Pattern = "bruteforce",
                Token = password.Substring(i, j - i + 1),
                i = i,
                j = j
            };
        }

        private static List<Match> Unwind(OptimalValues optimal, int n)
        {
            var optimalMatchSequence = new List<Match>();
            var k = n - 1;
            var l = -1;
            var g = double.PositiveInfinity;

            foreach (var candidateL in optimal.G[k].Keys)
            {
                var candidateG = optimal.G[k][candidateL];

                if (candidateG < g)
                {
                    l = candidateL;
                    g = candidateG;
                }
            }

            while (k >= 0)
            {
                var m = optimal.M[k][l];
                optimalMatchSequence.Insert(0, m);
                k = m.i - 1;
                l--;
            }

            return optimalMatchSequence;
        }

        private static void Update(string password, OptimalValues optimal, Match m, int l, bool excludeAdditive)
        {
            var k = m.j;
            var pi = EstimateGuesses(m, password);
            if (l > 1)
                pi *= optimal.Pi[m.i - 1][l - 1];

            var g = Factorial(l) * pi;
            if (!excludeAdditive)
                g += Math.Pow(MinimumGuessesBeforeGrowingSequence, l - 1);

            foreach (var competingL in optimal.G[k].Keys)
            {
                var competingG = optimal.G[k][competingL];
                if (competingL > l)
                    continue;
                if (competingG <= g)
                    return;
            }

            optimal.G[k][l] = g;
            optimal.M[k][l] = m;
            optimal.Pi[k][l] = pi;
        }
    }

    internal class MostGuessableMatchResult
    {
        public double Guesses { get; set; }
        public string Password { get; set; }
        public double Score { get; set; }
        public IEnumerable<Match> Sequence { get; set; }
    }

    internal class OptimalValues
    {
        public List<Dictionary<int, double>> G = new List<Dictionary<int, double>>();
        public List<Dictionary<int, double>> Pi = new List<Dictionary<int, double>>();

        public OptimalValues(int length)
        {
            for (var i = 0; i < length; i++)
            {
                G.Add(new Dictionary<int, double>());
                Pi.Add(new Dictionary<int, double>());
                M.Add(new Dictionary<int, Match>());
            }
        }

        public List<Dictionary<int, Match>> M { get; set; } = new List<Dictionary<int, Match>>();
    }
}
