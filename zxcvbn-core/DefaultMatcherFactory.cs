﻿using System.Collections.Generic;
using Zxcvbn.Matcher;

namespace Zxcvbn
{
    /// <inheritdoc />
    /// <summary>
    /// <para>This matcher factory will use all of the default password matchers.</para>
    /// <para>Default dictionary matchers use the built-in word lists: passwords, english, male_names, female_names, surnames</para>
    /// <para>Also matching against: user data, all dictionaries with l33t substitutions</para>
    /// <para>Other default matchers: repeats, sequences, digits, years, dates, spatial</para>
    /// <para>See <see cref="T:Zxcvbn.Matcher.IMatcher" /> and the classes that implement it for more information on each kind of pattern matcher.</para>
    /// </summary>
    internal class DefaultMatcherFactory : IMatcherFactory
    {
        private readonly List<IMatcher> _matchers;

        /// <summary>
        /// Create a matcher factory that uses the default list of pattern matchers
        /// </summary>
        public DefaultMatcherFactory()
        {
            var dictionaryMatchers = new List<IMatcher> {
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
                new ReverseDictionaryMatcher("us_tv_and_film", "us_tv_and_film.lst")
            };

            _matchers = new List<IMatcher> {
                new RepeatMatcher(),
                new SequenceMatcher(),
                new RegexMatcher("\\d{3,}", "digits"),
                new RegexMatcher("19\\d\\d|200\\d|201\\d", "year"),
                new DateMatcher(),
                new SpatialMatcher()
            };

            _matchers.AddRange(dictionaryMatchers);
            _matchers.Add(new L33tMatcher(dictionaryMatchers));
        }

        /// <inheritdoc />
        /// <summary>
        /// Get instances of pattern matchers, adding in per-password matchers on userInputs (and userInputs with l33t substitutions)
        /// </summary>
        /// <param name="userInputs">Enumerable of user information</param>
        /// <returns>Enumerable of matchers to use</returns>
        public IEnumerable<IMatcher> CreateMatchers(IEnumerable<string> userInputs)
        {
            var userInputDict = new DictionaryMatcher("user_inputs", userInputs);
            var leetUser = new L33tMatcher(userInputDict);

            return new List<IMatcher>(_matchers) { userInputDict, leetUser };
        }
    }
}
