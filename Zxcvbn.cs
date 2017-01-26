using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zxcvbn.Matcher;
using System.Text.RegularExpressions;

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
        private const string BruteforcePattern = "bruteforce";

        private IMatcherFactory matcherFactory;
        private readonly Translation translation;

        /// <summary>
        /// Create a new instance of Zxcvbn that uses the default matchers.
        /// </summary>
        public Zxcvbn(Translation translation = Translation.English)
            : this(new DefaultMatcherFactory())
        {
            this.translation = translation;
        }

        /// <summary>
        /// Create an instance of Zxcvbn that will use the given matcher factory to create matchers to use
        /// to find password weakness.
        /// </summary>
        /// <param name="matcherFactory">The factory used to create the pattern matchers used</param>
        /// <param name="translation">The language in which the strings are returned</param>
        public Zxcvbn(IMatcherFactory matcherFactory, Translation translation = Translation.English)
        {
            this.matcherFactory = matcherFactory;
            this.translation = translation;
        }

        /// <summary>
        /// <para>Perform the password matching on the given password and user inputs, returing the result structure with information
        /// on the lowest entropy match found.</para>
        /// 
        /// <para>User data will be treated as another kind of dictionary matching, but can be different for each password being evaluated.</para>para>
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="userInputs">Optionally, an enumarable of user data</param>
        /// <returns>Result for lowest entropy match</returns>
        public Result EvaluatePassword(string password, IEnumerable<string> userInputs = null)
        {
            userInputs = userInputs ?? new string[0];

            IEnumerable<Match> matches = new List<Match>();
            
            var timer = System.Diagnostics.Stopwatch.StartNew();
            
            foreach (var matcher in matcherFactory.CreateMatchers(userInputs))
            {
                matches = matches.Union(matcher.MatchPassword(password));
            }

            var result = FindMinimumEntropyMatch(password, matches);

            timer.Stop();
            result.CalcTime = timer.ElapsedMilliseconds;

            return result;
        }

        /// <summary>
        /// Returns a new result structure initialised with data for the lowest entropy result of all of the matches passed in, adding brute-force
        /// matches where there are no lesser entropy found pattern matches.
        /// </summary>
        /// <param name="matches">Password being evaluated</param>
        /// <param name="password">List of matches found against the password</param>
        /// <returns>A result object for the lowest entropy match sequence</returns>
        private Result FindMinimumEntropyMatch(string password, IEnumerable<Match> matches)
        {
            var bruteforce_cardinality = PasswordScoring.PasswordCardinality(password);

            // Minimum entropy up to position k in the password
            var minimumEntropyToIndex = new double[password.Length];
            var bestMatchForIndex = new Match[password.Length];
 
            for (var k = 0; k < password.Length; k++)
            {
                // Start with bruteforce scenario added to previous sequence to beat
                minimumEntropyToIndex[k] = (k == 0 ? 0 : minimumEntropyToIndex[k - 1]) + Math.Log(bruteforce_cardinality, 2);

                // All matches that end at the current character, test to see if the entropy is less
                foreach (var match in matches.Where(m => m.j == k))
                {
                    var candidate_entropy = (match.i <= 0 ? 0 : minimumEntropyToIndex[match.i - 1]) + match.Entropy;
                    if (candidate_entropy < minimumEntropyToIndex[k])
                    {
                        minimumEntropyToIndex[k] = candidate_entropy;
                        bestMatchForIndex[k] = match;
                    }
                }
            }


            // Walk backwards through lowest entropy matches, to build the best password sequence
            var matchSequence = new List<Match>();
            for (var k = password.Length - 1; k >= 0; k--)
            {
                if (bestMatchForIndex[k] != null)
                {
                    matchSequence.Add(bestMatchForIndex[k]);
                    k = bestMatchForIndex[k].i; // Jump back to start of match
                }
            }
            matchSequence.Reverse();


            // The match sequence might have gaps, fill in with bruteforce matching
            // After this the matches in matchSequence must cover the whole string (i.e. match[k].j == match[k + 1].i - 1)
            if (matchSequence.Count == 0)
            {
                // To make things easy, we'll separate out the case where there are no matches so everything is bruteforced
                matchSequence.Add(new Match()
                {
                    i = 0,
                    j = password.Length,
                    Token = password,
                    Cardinality = bruteforce_cardinality,
                    Pattern = BruteforcePattern,
                    Entropy = Math.Log(Math.Pow(bruteforce_cardinality, password.Length), 2)
                });
            }
            else
            {
                // There are matches, so find the gaps and fill them in
                var matchSequenceCopy = new List<Match>();
                for (var k = 0; k < matchSequence.Count; k++)
                {
                    var m1 = matchSequence[k];
                    var m2 = (k < matchSequence.Count - 1 ? matchSequence[k + 1] : new Match() { i = password.Length }); // Next match, or a match past the end of the password

                    matchSequenceCopy.Add(m1);
                    if (m1.j < m2.i - 1)
                    {
                        // Fill in gap
                        var ns = m1.j + 1;
                        var ne = m2.i - 1;
                        matchSequenceCopy.Add(new Match()
                        {
                            i = ns,
                            j = ne,
                            Token = password.Substring(ns, ne - ns + 1),
                            Cardinality = bruteforce_cardinality,
                            Pattern = BruteforcePattern,
                            Entropy = Math.Log(Math.Pow(bruteforce_cardinality, ne - ns + 1), 2)
                        });
                    }
                }

                matchSequence = matchSequenceCopy;
            }


            var minEntropy = (password.Length == 0 ? 0 : minimumEntropyToIndex[password.Length - 1]);
            var crackTime = PasswordScoring.EntropyToCrackTime(minEntropy);

            var result = new Result();
            result.Password = password;
            result.Entropy = Math.Round(minEntropy, 3);
            result.MatchSequence = matchSequence;
            result.CrackTime = Math.Round(crackTime, 3);
            result.CrackTimeDisplay = Utility.DisplayTime(crackTime, this.translation);
            result.Score = PasswordScoring.CrackTimeToScore(crackTime);


            //starting feedback
            if ((matchSequence == null) || (matchSequence.Count() == 0))
            {
                result.warning = Warning.Default;
                result.suggestions.Clear();
                result.suggestions.Add(Suggestion.Default);
            }
            else
            {
                //no Feedback if score is good or great
                if (result.Score > 2)
                {
                    result.warning = Warning.Empty;
                    result.suggestions.Clear();
                    result.suggestions.Add(Suggestion.Empty);
                }
                else
                {
                    //tie feedback to the longest match for longer sequences                   
                    Match longestMatch = GetLongestMatch(matchSequence);
                    GetMatchFeedback(longestMatch, matchSequence.Count() == 1, result);
                    result.suggestions.Insert(0,Suggestion.AddAnotherWordOrTwo);
                }


            }
            return result;
        }

        private Match GetLongestMatch(List<Match> matchSequence)
        {
            Match longestMatch;

            if ((matchSequence != null) && (matchSequence.Count() > 0))
            {
                longestMatch = matchSequence[0];
                foreach (Match match in matchSequence)
                {
                    if (match.Token.Length > longestMatch.Token.Length)
                        longestMatch = match;
                }
            }
            else
                longestMatch = new Match();

            return longestMatch;
        }

        private void GetMatchFeedback(Match match, bool isSoleMatch, Result result)
        {
            switch (match.Pattern)
            {
                case "dictionary":
                    GetDictionaryMatchFeedback((DictionaryMatch)match, isSoleMatch, result);
                break;

                case "spatial":
                    SpatialMatch spatialMatch = (SpatialMatch) match;

                    if (spatialMatch.Turns == 1)
                        result.warning = Warning.StraightRow;
                    else
                        result.warning = Warning.ShortKeyboardPatterns;

                    result.suggestions.Clear();
                    result.suggestions.Add(Suggestion.UseLongerKeyboardPattern);
                    break;

                case "repeat":
                    //todo: add support for repeated sequences longer than 1 char
                  //  if(match.Token.Length == 1)
                        result.warning = Warning.RepeatsLikeAaaEasy;
                  //  else
                 //       result.warning = Warning.RepeatsLikeAbcSlighterHarder;

                    result.suggestions.Clear();
                    result.suggestions.Add(Suggestion.AvoidRepeatedWordsAndChars);
                    break;

                case "sequence":
                    result.warning = Warning.SequenceAbcEasy;

                    result.suggestions.Clear();
                    result.suggestions.Add(Suggestion.AvoidSequences);
                    break;

                //todo: add support for recent_year, however not example exist on https://dl.dropboxusercontent.com/u/209/zxcvbn/test/index.html


                case "date":
                    result.warning = Warning.DatesEasy;

                    result.suggestions.Clear();
                    result.suggestions.Add(Suggestion.AvoidDatesYearsAssociatedYou);
                    break;
            }
        }

        private void GetDictionaryMatchFeedback(DictionaryMatch match, bool isSoleMatch, Result result)
        {
            if (match.DictionaryName.Equals("passwords"))
            {
                //todo: add support for reversed words
                if (isSoleMatch == true && !(match is L33tDictionaryMatch))
                {
                        if (match.Rank <= 10)
                            result.warning = Warning.Top10Passwords;
                        else if (match.Rank <= 100)
                            result.warning = Warning.Top100Passwords;
                        else
                            result.warning = Warning.CommonPasswords;
                }
                else if (PasswordScoring.CrackTimeToScore(PasswordScoring.EntropyToCrackTime(match.Entropy)) <= 1)
                {
                    result.warning = Warning.SimilarCommonPasswords;
                }
            }
            else if (match.DictionaryName.Equals("english"))
            {
                if (isSoleMatch == true)
                    result.warning = Warning.WordEasy;
            }
            else if (match.DictionaryName.Equals("surnames") ||
                     match.DictionaryName.Equals("male_names") ||
                     match.DictionaryName.Equals("female_names"))
            {
                if (isSoleMatch == true)
                    result.warning = Warning.NameSurnamesEasy;
                else
                    result.warning = Warning.CommonNameSurnamesEasy;
            }
            else
            {
                result.warning = Warning.Empty;
            }

            string word = match.Token;
            if (Regex.IsMatch(word, PasswordScoring.StartUpper))
            {
                result.suggestions.Add(Suggestion.CapsDontHelp);
            }
            else if (Regex.IsMatch(word, PasswordScoring.AllUpper) && !word.Equals(word.ToLowerInvariant()))
            {
                result.suggestions.Add(Suggestion.AllCapsEasy);
            }

            //todo: add support for reversed words
            //if match.reversed and match.token.length >= 4
            //    suggestions.push "Reversed words aren't much harder to guess"

            if (match is L33tDictionaryMatch)
            {
                result.suggestions.Add(Suggestion.PredictableSubstitutionsEasy);
            }
        }

        /// <summary>
        /// <para>A static function to match a password against the default matchers without having to create
        /// an instance of Zxcvbn yourself, with supplied user data. </para>
        /// 
        /// <para>Supplied user data will be treated as another kind of dictionary matching.</para>
        /// </summary>
        /// <param name="password">the password to test</param>
        /// <param name="userInputs">optionally, the user inputs list</param>
        /// <returns>The results of the password evaluation</returns>
        public static Result MatchPassword(string password, IEnumerable<string> userInputs = null)
        {
            var zx = new Zxcvbn(new DefaultMatcherFactory());
            return zx.EvaluatePassword(password, userInputs);
        }

    }
}
