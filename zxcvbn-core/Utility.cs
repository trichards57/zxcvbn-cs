using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Zxcvbn
{
    /// <summary>
    /// A few useful extension methods used through the Zxcvbn project.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Returns a list of the lines of text from an embedded resource in the assembly.
        /// </summary>
        /// <param name="resourceName">The name of the resource to get the contents from.</param>
        /// <returns>An enumerable of lines of text in the resource or null if the resource does not exist.</returns>
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
        /// Reverse a string in one call.
        /// </summary>
        /// <param name="str">String to reverse.</param>
        /// <returns>String in reverse.</returns>
        public static string StringReverse(this string str)
        {
            return new string(str.Reverse().ToArray());
        }
    }
}
