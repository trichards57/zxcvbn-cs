namespace Zxcvbn
{
    /// <summary>
    /// Warning associated with the password analysis
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
}