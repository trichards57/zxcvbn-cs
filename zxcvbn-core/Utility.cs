using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Zxcvbn
{
    /// <summary>
    /// A few useful extension methods used through the Zxcvbn project
    /// </summary>
    internal static class Utility
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
            long minute = 60, hour = minute * 60, day = hour * 24, month = day * 31, year = month * 12, century = year * 100;

            if (seconds < minute) return GetTranslation("instant", translation);
            else if (seconds < hour) return string.Format("{0} " + GetTranslation("minutes", translation), (1 + Math.Ceiling(seconds / minute)));
            else if (seconds < day) return string.Format("{0} " + GetTranslation("hours", translation), (1 + Math.Ceiling(seconds / hour)));
            else if (seconds < month) return string.Format("{0} " + GetTranslation("days", translation), (1 + Math.Ceiling(seconds / day)));
            else if (seconds < year) return string.Format("{0} " + GetTranslation("months", translation), (1 + Math.Ceiling(seconds / month)));
            else if (seconds < century) return string.Format("{0} " + GetTranslation("years", translation), (1 + Math.Ceiling(seconds / year)));
            else return GetTranslation("centuries", translation);
        }

        /// <summary>
        /// Shortcut for string.Format
        /// </summary>
        /// <param name="args">Format args</param>
        /// <param name="str">Format string</param>
        /// <returns>Formatted string</returns>
        public static string F(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// Returns a list of the lines of text from an embedded resource in the assembly.
        /// </summary>
        /// <param name="resourceName">The name of the resource to get the contents of</param>
        /// <returns>An enumerable of lines of text in the resource or null if the resource does not exist</returns>
        public static IEnumerable<string> GetEmbeddedResourceLines(string resourceName)
        {
            var asm = typeof(Utility).GetTypeInfo().Assembly;
            if (!asm.GetManifestResourceNames().Contains(resourceName)) return null; // Not an embedded resource

            var lines = new List<string>();

            using (var stream = asm.GetManifestResourceStream(resourceName))
            using (var text = new StreamReader(stream))
            {
                while (!text.EndOfStream)
                {
                    lines.Add(text.ReadLine());
                }
            }

            return lines;
        }

        /// <summary>
        /// A convenience for parsing a substring as an int and returning the results. Uses TryParse, and so returns zero where there is no valid int
        /// </summary>
        /// <param name="r">Substring parsed as int or zero</param>
        /// <param name="length">Length of substring to parse</param>
        /// <param name="startIndex">Start index of substring to parse</param>
        /// <param name="str">String to get substring of</param>
        /// <returns>True if the parse succeeds</returns>
        public static bool IntParseSubstring(this string str, int startIndex, int length, out int r)
        {
            return int.TryParse(str.Substring(startIndex, length), out r);
        }

        /// <summary>
        /// Reverse a string in one call
        /// </summary>
        /// <param name="str">String to reverse</param>
        /// <returns>String in reverse</returns>
        public static string StringReverse(this string str)
        {
            return new string(str.Reverse().ToArray());
        }

        /// <summary>
        /// Quickly convert a string to an integer, uses TryParse so any non-integers will return zero
        /// </summary>
        /// <param name="str">String to parse into an int</param>
        /// <returns>Parsed int or zero</returns>
        public static int ToInt(this string str)
        {
            int r = 0;
            int.TryParse(str, out r);
            return r;
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