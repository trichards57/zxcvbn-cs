﻿namespace Zxcvbn.Matcher.Matches
{
    /// <inheritdoc />
    /// <summary>
    /// A match made using the <see cref="T:Zxcvbn.Matcher.SequenceMatcher" /> containing some additional sequence information.
    /// </summary>
    internal class SequenceMatch : Match
    {
        /// <summary>
        /// Whether the match was found in ascending order (cdefg) or not (zyxw)
        /// </summary>
        public bool Ascending { get; set; }

        /// <summary>
        /// The name of the sequence that the match was found in (e.g. 'lower', 'upper', 'digits')
        /// </summary>
        public string SequenceName { get; set; }

        public int SequenceSpace { get; set; }
    }
}
