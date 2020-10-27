﻿namespace Zxcvbn.Matcher.Matches
{
    /// <inheritdoc />
    /// <summary>
    /// A match made using the <see cref="T:Zxcvbn.Matcher.SequenceMatcher" /> containing some additional sequence information.
    /// </summary>
    public class SequenceMatch : Match
    {
        /// <summary>
        /// Whether the match was found in ascending order (cdefg) or not (zyxw)
        /// </summary>
        public bool Ascending { get; set; }

        public long BaseGuesses { get; set; }
        public string BaseMatches { get; set; }
        public string BaseToken { get; set; }
        public long RepeatCount { get; set; }
        public string Sequence { get; set; }

        /// <summary>
        /// The name of the sequence that the match was found in (e.g. 'lower', 'upper', 'digits')
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        /// The size of the sequence the match was found in (e.g. 26 for lowercase letters)
        /// </summary>
        public int SequenceSize { get; set; }

        public int SequenceSpace { get; set; }
    }
}
