using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zxcvbn.Matcher.Matches
{
    /// <summary>
    /// A match identified as being in one of the provided dictionaries.
    /// </summary>
    /// <seealso cref="Zxcvbn.Matcher.Matches.Match" />
    public class DictionaryMatch : Match
    {
        /// <summary>
        /// Gets the base guesses associated with the matched word.
        /// </summary>
        public long BaseGuesses { get; internal set; }

        /// <summary>
        /// Gets the name of the dictionary containing the matched word.
        /// </summary>
        public string DictionaryName { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the matched word was found with l33t spelling.
        /// </summary>
        public bool L33t { get; internal set; }

        /// <summary>
        /// Gets the number of L33T variations associated with this match.
        /// </summary>
        public long L33tVariations { get; internal set; }

        /// <summary>
        /// Gets the dictionary word matched to.
        /// </summary>
        public string MatchedWord { get; internal set; }

        /// <summary>
        /// Gets the name of the pattern matcher used to generate this match.
        /// </summary>
        public override string Pattern => "dictionary";

        /// <summary>
        /// Gets the rank of the matched word in the dictionary.
        /// </summary>
        /// <remarks>
        /// The most frequent word is has a rank of 1, with less frequent words having higher ranks.
        /// </remarks>
        public int Rank { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the matched word was reversed.
        /// </summary>
        public bool Reversed { get; internal set; }

        /// <summary>
        /// Gets the l33t character mappings that are in use for this match.
        /// </summary>
        public IReadOnlyDictionary<char, char> Sub => new ReadOnlyDictionary<char, char>(L33tSubs);

        /// <summary>
        /// Gets the number of uppercase variations associated with this match.
        /// </summary>
        public long UppercaseVariations { get; internal set; }

        /// <summary>
        /// Gets or sets the l33t character mappings that are in use for this match.
        /// </summary>
        /// <remarks>
        /// Modifiable version of Sub.
        /// </remarks>
        internal Dictionary<char, char> L33tSubs { get; set; } = new Dictionary<char, char>();
    }
}
