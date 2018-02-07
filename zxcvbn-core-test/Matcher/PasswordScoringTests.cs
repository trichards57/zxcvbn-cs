//using FluentAssertions;
//using Xunit;

//namespace Zxcvbn.Tests.Matcher
//{
//    public class PasswordScoringTests
//    {
//        [Theory(DisplayName = "PasswordScoring.Binomial"),
//         InlineData(0, 0, 1),
//         InlineData(1, 0, 1),
//         InlineData(0, 1, 0),
//         InlineData(1, 1, 1),
//         InlineData(8, 3, 56),
//         InlineData(52, 5, 2598960)]
//        public void PasswordScoringBinomialScoresCorrectly(int n, int k, int score)
//        {
//            PasswordScoring.Binomial(n, k).Should().Be(score);
//        }

//        [Theory(DisplayName = "PasswordScoring.PasswordCardinality"),
//                 InlineData("asdf", 26),
//         InlineData("ASDF", 26),
//         InlineData("aSDf", 52),
//         InlineData("124890", 10),
//         InlineData("aS159Df", 62),
//         InlineData("!@<%:{$:#<@}{+&)(*%", 33),
//         InlineData("©", 100),
//         InlineData("ThisIs@T3stP4ssw0rd!", 95)]
//        public void PasswordScoringPasswordCardinalityScoresCorrectly(string password, int score)
//        {
//            PasswordScoring.PasswordCardinality(password).Should().Be(score);
//        }
//    }
//}