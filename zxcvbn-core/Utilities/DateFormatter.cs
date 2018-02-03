using System;

namespace Zxcvbn.Utilities
{
    public static class DateFormatter
    {
        /// <summary>
        /// Convert a number of seconds into a human readable form. Rounds up.
        /// To be consistent with zxcvbn, it returns the unit + 1 (i.e. 60 * 10 seconds = 10 minutes would come out as "11 minutes"
        /// this is probably to avoid ever needing to deal with plurals
        /// </summary>
        /// <param name="seconds">The time in seconds</param>
        /// <param name="translation">The language in which the string is returned</param>
        /// <returns>A human-friendly time string</returns>
        public static string DisplayTime(double seconds, Translation translation = Translation.English)
        {
            const long minute = 60;
            const long hour = minute * 60;
            const long day = hour * 24;
            const long month = day * 31;
            const long year = month * 12;
            const long century = year * 100;

            if (seconds < minute) return GetTranslation("instant", translation);
            if (seconds < hour) return string.Format("{0} " + GetTranslation("minutes", translation), 1 + Math.Ceiling(seconds / minute));
            if (seconds < day) return string.Format("{0} " + GetTranslation("hours", translation), 1 + Math.Ceiling(seconds / hour));
            if (seconds < month) return string.Format("{0} " + GetTranslation("days", translation), 1 + Math.Ceiling(seconds / day));
            if (seconds < year) return string.Format("{0} " + GetTranslation("months", translation), 1 + Math.Ceiling(seconds / month));
            if (seconds < century) return string.Format("{0} " + GetTranslation("years", translation), 1 + Math.Ceiling(seconds / year));
            return GetTranslation("centuries", translation);
        }

        private static string GetTranslation(string matcher, Translation translation)
        {
            string translated;

            switch (matcher)
            {
                case "instant":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "unmittelbar";
                            break;

                        case Translation.France:
                            translated = "instantané";
                            break;

                        default:
                            translated = "instant";
                            break;
                    }
                    break;

                case "minutes":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "Minuten";
                            break;

                        case Translation.France:
                            translated = "Minutes";
                            break;

                        default:
                            translated = "minutes";
                            break;
                    }
                    break;

                case "hours":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "Stunden";
                            break;

                        case Translation.France:
                            translated = "Heures";
                            break;

                        default:
                            translated = "hours";
                            break;
                    }
                    break;

                case "days":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "Tage";
                            break;

                        case Translation.France:
                            translated = "Journées";
                            break;

                        default:
                            translated = "days";
                            break;
                    }
                    break;

                case "months":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "Monate";
                            break;

                        case Translation.France:
                            translated = "Mois";
                            break;

                        default:
                            translated = "months";
                            break;
                    }
                    break;

                case "years":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "Jahre";
                            break;

                        case Translation.France:
                            translated = "Ans";
                            break;

                        default:
                            translated = "years";
                            break;
                    }
                    break;

                case "centuries":
                    switch (translation)
                    {
                        case Translation.German:
                            translated = "Jahrhunderte";
                            break;

                        case Translation.France:
                            translated = "Siècles";
                            break;

                        default:
                            translated = "centuries";
                            break;
                    }
                    break;

                default:
                    translated = matcher;
                    break;
            }

            return translated;
        }
    }
}