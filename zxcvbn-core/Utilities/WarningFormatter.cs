using System;

namespace Zxcvbn.Utilities
{
    public static class WarningFormatter
    {
        /// <summary>
        /// Get a translated string of the Warning
        /// </summary>
        /// <param name="warning">Warning enum to get the string from</param>
        /// <param name="translation">Language in which to return the string to. Default is English.</param>
        /// <returns>Warning string in the right language</returns>
        public static string GetWarning(Warning warning, Translation translation = Translation.English)
        {
            string translated;

            if (translation != Translation.English)
            {
                throw new NotImplementedException("Translating warnings into other languages is not yet supported.");
            }

            switch (warning)
            {
                case Warning.StraightRow:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Straight rows of keys are easy to guess";
                            break;
                    }
                    break;

                case Warning.ShortKeyboardPatterns:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Short keyboard patterns are easy to guess";
                            break;
                    }
                    break;

                case Warning.RepeatsLikeAaaEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Repeats like \"aaa\" are easy to guess";
                            break;
                    }
                    break;

                case Warning.RepeatsLikeAbcSlighterHarder:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Repeats like \"abcabcabc\" are only slightly harder to guess than \"abc\"";
                            break;
                    }
                    break;

                case Warning.SequenceAbcEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Sequences like abc or 6543 are easy to guess";
                            break;
                    }
                    break;

                case Warning.RecentYearsEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Recent years are easy to guess";
                            break;
                    }
                    break;

                case Warning.DatesEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Dates are often easy to guess";
                            break;
                    }
                    break;

                case Warning.Top10Passwords:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "This is a top-10 common password";
                            break;
                    }
                    break;

                case Warning.Top100Passwords:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "This is a top-100 common password";
                            break;
                    }
                    break;

                case Warning.CommonPasswords:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "This is a very common password";
                            break;
                    }
                    break;

                case Warning.SimilarCommonPasswords:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "This is similar to a commonly used password";
                            break;
                    }
                    break;

                case Warning.WordEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "A word by itself is easy to guess";
                            break;
                    }
                    break;

                case Warning.NameSurnamesEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Names and surnames by themselves are easy to guess";
                            break;
                    }
                    break;

                case Warning.CommonNameSurnamesEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Common names and surnames are easy to guess";
                            break;
                    }
                    break;

                case Warning.Empty:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "";
                            break;
                    }
                    break;

                default:
                    translated = "";
                    break;
            }
            return translated;
        }
    }
}