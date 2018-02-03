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

    }
}