namespace Zxcvbn
{
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
}