using FluentAssertions;
using Xunit;

namespace Zxcvbn.Tests
{
    public class ZxcvbnTests
    {
        [Fact]
        public void EmptyPassword()
        {
            Zxcvbn.MatchPassword("").Entropy.Should().Be(0);
        }

        //[Theory,
        //InlineData("zxcvbn", 0, 1.76343, Warning.Top100Passwords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("qwER43@!", 2, 7.95651, Warning.ShortKeyboardPatterns, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.UseLongerKeyboardPattern }),
        ////InlineData("Tr0ub4dour&3", 2, 7.28008, Warning.Top100Passwords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("correcthorsebatterystaple", 4, 14.43696, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("coRrecth0rseba++ery9.23.2007staple$", 4, 20.71185, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("D0g..................", 1, 5.64542, Warning.RepeatsLikeAaaEasy, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.AvoidRepeatedWordsAndChars }),
        ////InlineData("abcdefghijk987654321", 1, 4.17609, Warning.SequenceAbcEasy, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.AvoidSequences }),
        ////InlineData("neverforget13/3/1997", 3, 9.55517, Warning.Empty, new[] { Suggestion.Empty }),
        //InlineData("1qaz2wsx3edc", 0, 3.00043, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("temppass22", 1, 5.5887, Warning.SimilarCommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("briansmith", 1, 4.17609, Warning.CommonNameSurnamesEasy, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("briansmith4mayor", 4, 10.17898, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("password1", 0, 2.27875, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        //InlineData("viking", 0, 2.38739, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        //InlineData("thx1138", 0, 2.32015, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("ScoRpi0ns", 1, 5.46189, Warning.Empty, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.PredictableSubstitutionsEasy }),
        ////InlineData("do you know", 3, 9, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("ryanhunter2000", 3, 8.00325, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("rianhunter2000", 3, 8.39794, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("asdfghju7654rewq", 3, 8.96529, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("AOEUIDHG&*()LS_", 3, 9.10726, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("12345678", 0, 0.60206, Warning.Top10Passwords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("defghi6789", 1, 4.40824, Warning.SequenceAbcEasy, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.AvoidSequences }),
        //InlineData("rosebud", 0, 2.43457, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        //InlineData("Rosebud", 0, 2.7348, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.CapsDontHelp }),
        //InlineData("ROSEBUD", 0, 2.7348, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.AllCapsEasy }),
        //InlineData("rosebuD", 0, 2.7348, Warning.CommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo }),
        ////InlineData("ros3bud99", 1, 4.80754, Warning.SimilarCommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.PredictableSubstitutionsEasy }),
        ////InlineData("r0s3bud99", 1, 5.07335, Warning.SimilarCommonPasswords, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.PredictableSubstitutionsEasy }),
        ////InlineData("R0$38uD99", 2, 6.11754, Warning.Empty, new[] { Suggestion.AddAnotherWordOrTwo, Suggestion.PredictableSubstitutionsEasy }),
        ////InlineData("verlineVANDERMARK", 4, 10.38918, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("eheuczkqyq", 3, 10, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("rWibMFACxAUGZmxhVncy", 4, 20, Warning.Empty, new[] { Suggestion.Empty }),
        ////InlineData("Ba9ZyWABu99[BK#6MBgbH88Tofv)vs$w", 4, 32, Warning.Empty, new[] { Suggestion.Empty })
        //]
        //public void MatchesZxcvbn(string password, int score, double guessLog10, Warning expectedWarning, IEnumerable<Suggestion> expectedSuggestions)
        //{
        //    var zx = Zxcvbn.MatchPassword(password);

        //    var guesses = Math.Pow(2, zx.Entropy);
        //    var actualGuessLog10 = Math.Log10(guesses);

        //    zx.Score.Should().Be(score);
        //    actualGuessLog10.Should().BeApproximately(guessLog10, guessLog10 / 100);
        //    zx.Warning.Should().Be(expectedWarning);
        //    zx.Suggestions.ShouldAllBeEquivalentTo(expectedSuggestions);
        //}
    }
}