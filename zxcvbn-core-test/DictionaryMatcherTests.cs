using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;

namespace Zxcvbn.Tests
{
    public class DictionaryMatcherTests
    {
        [Theory,
         InlineData("NotInDictionary", 0),
         InlineData("choreography", 1),
         InlineData("ChOrEograPHy", 1)]
        public void DictionaryMatch(string word, int count)
        {
            var dm = new DictionaryMatcher("test", "test_dictionary.txt");
            dm.MatchPassword(word).Should().HaveCount(count);
        }
    }
}