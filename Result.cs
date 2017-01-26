using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: These should probably be immutable
namespace Zxcvbn
{
    /// <summary>
    /// SWarning associated with the password analysis
    /// </summary>
    public enum Warning
    {
        /// <summary>
        /// Empty string
        /// </summary>
        Default,

        /// <summary>
        /// Straight rows of keys are easy to guess
        /// </summary>
        StraightRow,

        /// <summary>
        /// Short keyboard patterns are easy to guess
        /// </summary>
        ShortKeyboardPatterns,

        /// <summary>
        /// Repeats like "aaa" are easy to guess
        /// </summary>
        RepeatsLikeAaaEasy,

        /// <summary>
        /// Repeats like "abcabcabc" are only slightly harder to guess than "abc"
        /// </summary>
        RepeatsLikeAbcSlighterHarder,

        /// <summary>
        /// Sequences like abc or 6543 are easy to guess
        /// </summary>
        SequenceAbcEasy,

        /// <summary>
        /// Recent years are easy to guess
        /// </summary>
        RecentYearsEasy,

        /// <summary>
        ///  Dates are often easy to guess
        /// </summary>
        DatesEasy,

        /// <summary>
        ///  This is a top-10 common password
        /// </summary>
        Top10Passwords,

        /// <summary>
        /// This is a top-100 common password
        /// </summary>
        Top100Passwords,

        /// <summary>
        /// This is a very common password
        /// </summary>
        CommonPasswords,

        /// <summary>
        /// This is similar to a commonly used password
        /// </summary>
        SimilarCommonPasswords,

        /// <summary>
        /// A word by itself is easy to guess
        /// </summary>
        WordEasy,

        /// <summary>
        /// Names and surnames by themselves are easy to guess
        /// </summary>
        NameSurnamesEasy,

        /// <summary>
        /// Common names and surnames are easy to guess
        /// </summary>
        CommonNameSurnamesEasy,

        /// <summary>
        ///  Empty String
        /// </summary>
        Empty,

    }

    /// <summary>
    /// Suggestion on how to improve the password base on zxcvbn's password analysis
    /// </summary>
    public enum Suggestion
    {
        /// <summary>
        ///  Use a few words, avoid common phrases
        ///  No need for symbols, digits, or uppercase letters
        /// </summary>
        Default,

        /// <summary>
        ///  Add another word or two. Uncommon words are better.
        /// </summary>
        AddAnotherWordOrTwo,

        /// <summary>
        ///  Use a longer keyboard pattern with more turns
        /// </summary>
        UseLongerKeyboardPattern,

        /// <summary>
        ///  Avoid repeated words and characters
        /// </summary>
        AvoidRepeatedWordsAndChars,

        /// <summary>
        ///  Avoid sequences  
        /// </summary>
        AvoidSequences,

        /// <summary>
        ///  Avoid recent years
        ///  Avoid years that are associated with you
        /// </summary>
        AvoidYearsAssociatedYou,

        /// <summary>
        ///  Avoid dates and years that are associated with you 
        /// </summary>
        AvoidDatesYearsAssociatedYou,

        /// <summary>
        ///  Capitalization doesn't help very much  
        /// </summary>
        CapsDontHelp,

        /// <summary>
        /// All-uppercase is almost as easy to guess as all-lowercase   
        /// </summary>
        AllCapsEasy,

        /// <summary>
        /// Reversed words aren't much harder to guess   
        /// </summary>
        ReversedWordEasy,

        /// <summary>
        ///  Predictable substitutions like '@' instead of 'a' don't help very much  
        /// </summary>
        PredictableSubstitutionsEasy,

        /// <summary>
        ///  Empty String
        /// </summary>
        Empty,

    }





    /// <summary>
    /// The results of zxcvbn's password analysis
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Result constructor initialize Suggestion list.
        /// </summary>
        public Result() { suggestions = new List<Suggestion>(); }

        /// <summary>
        /// A calculated estimate of how many bits of entropy the password covers, rounded to three decimal places.
        /// </summary>
        public double Entropy { get; set; }

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
        /// A score from 0 to 4 (inclusive), with 0 being least secure and 4 being most secure calculated from crack time:
        /// [0,1,2,3,4] if crack time is less than [10**2, 10**4, 10**6, 10**8, Infinity] seconds.
        /// Useful for implementing a strength meter
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// The sequence of matches that were used to create the entropy calculation
        /// </summary>
        public IList<Match> MatchSequence { get; set; }

        /// <summary>
        /// The password that was used to generate these results
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Warning on this password
        /// </summary>
        public Warning  warning {get; set;}

        /// <summary>
        /// Suggestion on how to improve the password
        /// </summary>
        public List<Suggestion> suggestions { get; set; }


    }

    /// <summary>
    /// <para>A single match that one of the pattern matchers has made against the password being tested.</para>
    /// 
    /// <para>Some pattern matchers implement subclasses of match that can provide more information on their specific results.</para>
    /// 
    /// <para>Matches must all have the <see cref="Pattern"/>, <see cref="Token"/>, <see cref="Entropy"/>, <see cref="i"/> and
    /// <see cref="j"/> fields (i.e. all but the <see cref="Cardinality"/> field, which is optional) set before being returned from the matcher
    /// in which they are created.</para>
    /// </summary>
    public class Match
    {
        /// <summary>
        /// The name of the pattern matcher used to generate this match
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The portion of the password that was matched
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The entropy that this portion of the password covers using the current pattern matching technique
        /// </summary>
        public double Entropy { get; set; }


        // The following are more internal measures, but may be useful to consumers

        /// <summary>
        /// Some pattern matchers can associate the cardinality of the set of possible matches that the 
        /// entropy calculation is derived from. Not all matchers provide a value for cardinality.
        /// </summary>
        public int Cardinality { get; set; } 

        /// <summary>
        /// The start index in the password string of the matched token. 
        /// </summary>
        public int i { get; set; } // Start Index

        /// <summary>
        /// The end index in the password string of the matched token.
        /// </summary>
        public int j { get; set; } // End Index
    }

}
