using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class ReverseDictionaryMatcherTests
    {
        private readonly ReverseDictionaryMatcher _matcher = new ReverseDictionaryMatcher("d1", "test_dictionary_3.txt");

        [Fact]
        public void MatchesAganstReversedWords()
        {
            var result = _matcher.MatchPassword("0123456789").OfType<DictionaryMatch>().ToList();

            var expected = new[] { new DictionaryMatch
            {
                DictionaryName = "d1",
                i = 1,
                j = 3,
                MatchedWord = "321",
                Rank = 2,
                Reversed = true,
                L33t = false,
                Token = "123"
            } ,
                new DictionaryMatch{
                DictionaryName = "d1",
                i = 4,
                j = 6,
                MatchedWord = "654",
                Rank = 4,
                Reversed = true,
                L33t = false,
                Token = "456"
            }};
            result.Should().BeEquivalentTo(expected);
        }
    }
}
