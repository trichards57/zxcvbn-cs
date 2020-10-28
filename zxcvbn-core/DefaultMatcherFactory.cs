using System.Collections.Generic;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <summary>
    /// Creates the default matchers.
    /// </summary>
    internal static class DefaultMatcherFactory
    {
        private static readonly IEnumerable<IMatcher> BuiltInMatchers = BuildBuiltInMatchers();

        /// <summary>
        /// Gets all of the built in matchers, as well as matchers for the custom dictionaries.
        /// </summary>
        /// <param name="userInputs">Enumerable of user information.</param>
        /// <returns>Enumerable of matchers to use.</returns>
        public static IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs)
        {
            var userInputDict = new DictionaryMatcher("user_inputs", userInputs);
            var leetUser = new L33tMatcher(userInputDict);

            return new List<IMatcher>(BuiltInMatchers) { userInputDict, leetUser };
        }

        private static IEnumerable<IMatcher> BuildBuiltInMatchers()
        {
            var dictionaryMatchers = new List<IMatcher>
            {
                new DictionaryMatcher("passwords", "passwords.lst"),
                new DictionaryMatcher("english", "english.lst"),
                new DictionaryMatcher("male_names", "male_names.lst"),
                new DictionaryMatcher("female_names", "female_names.lst"),
                new DictionaryMatcher("surnames", "surnames.lst"),
                new DictionaryMatcher("us_tv_and_film", "us_tv_and_film.lst"),
                new ReverseDictionaryMatcher("passwords", "passwords.lst"),
                new ReverseDictionaryMatcher("english", "english.lst"),
                new ReverseDictionaryMatcher("male_names", "male_names.lst"),
                new ReverseDictionaryMatcher("female_names", "female_names.lst"),
                new ReverseDictionaryMatcher("surnames", "surnames.lst"),
                new ReverseDictionaryMatcher("us_tv_and_film", "us_tv_and_film.lst"),
            };

            return new List<IMatcher>(dictionaryMatchers)
            {
                new RepeatMatcher(),
                new SequenceMatcher(),
                new RegexMatcher("19\\d\\d|200\\d|201\\d", "recent_year"),
                new DateMatcher(),
                new SpatialMatcher(),
                new L33tMatcher(dictionaryMatchers),
            };
        }
    }
}
