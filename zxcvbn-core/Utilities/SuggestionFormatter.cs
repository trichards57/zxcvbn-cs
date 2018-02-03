using System;

namespace Zxcvbn.Utilities
{
    public static class SuggestionFormatter
    {
        /// <summary>
        /// Get a translated string of the Warning
        /// </summary>
        /// <param name="suggestion">Suggestion enum to get the string from</param>
        /// <param name="translation">Language in which to return the string to. Default is English.</param>
        /// <returns>Suggestion string in the right language</returns>
        public static string GetSuggestion(Suggestion suggestion, Translation translation = Translation.English)
        {
            string translated;

            if (translation != Translation.English)
            {
                throw new NotImplementedException("Translating warnings into other languages is not yet supported.");
            }

            switch (suggestion)
            {
                case Suggestion.AddAnotherWordOrTwo:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Add another word or two. Uncommon words are better.";
                            break;
                    }
                    break;

                case Suggestion.UseLongerKeyboardPattern:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Use a longer keyboard pattern with more turns";
                            break;
                    }
                    break;

                case Suggestion.AvoidRepeatedWordsAndChars:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Avoid repeated words and characters";
                            break;
                    }
                    break;

                case Suggestion.AvoidSequences:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Avoid sequences";
                            break;
                    }
                    break;

                case Suggestion.AvoidYearsAssociatedYou:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Avoid recent years \n Avoid years that are associated with you";
                            break;
                    }
                    break;

                case Suggestion.AvoidDatesYearsAssociatedYou:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Avoid dates and years that are associated with you";
                            break;
                    }
                    break;

                case Suggestion.CapsDontHelp:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Capitalization doesn't help very much";
                            break;
                    }
                    break;

                case Suggestion.AllCapsEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "All-uppercase is almost as easy to guess as all-lowercase";
                            break;
                    }
                    break;

                case Suggestion.ReversedWordEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Reversed words aren't much harder to guess";
                            break;
                    }
                    break;

                case Suggestion.PredictableSubstitutionsEasy:
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "";
                            break;

                        case Translation.France:
                            translated = "";
                            break;

                        default:
                            translated = "Predictable substitutions like '@' instead of 'a' don't help very much";
                            break;
                    }
                    break;

                case Suggestion.Empty:
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
                    translated = "Use a few words, avoid common phrases \n No need for symbols, digits, or uppercase letters";
                    break;
            }
            return translated;
        }
    }
}