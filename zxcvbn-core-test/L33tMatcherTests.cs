using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;

namespace Zxcvbn.Tests
{
    // ReSharper disable once InconsistentNaming
    public class L33tMatcherTests
    {
        [Theory, InlineData("3mu", 1)]
        public void DictionaryMatch(string word, int count)
        {
            var dm = new DictionaryMatcher("test", "test_dictionary.txt");
            var leet = new L33tMatcher(dm);
            leet.MatchPassword(word).Should().HaveCount(count);
        }
    }
}