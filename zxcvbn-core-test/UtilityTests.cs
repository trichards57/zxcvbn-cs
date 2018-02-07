//using FluentAssertions;
//using Xunit;
//using Zxcvbn.Utilities;

//namespace Zxcvbn.Tests
//{
//    public class UtilityTests
//    {
//        [Theory,
//         InlineData(60 * 10, "11 minutes"),
//         InlineData(60 * 60 * 24, "2 days"),
//         InlineData(60 * 60 * 24 * 365 * 15.4, "17 years")]
//        public void TimeDisplayStrings(double seconds, string result)
//        {
//            DateFormatter.DisplayTime(seconds).Should().Be(result);
//        }

//        [Theory,
//         InlineData(60 * 10, "11 Minuten"),
//         InlineData(60 * 60 * 24, "2 Tage"),
//         InlineData(60 * 60 * 24 * 365 * 15.4, "17 Jahre")]
//        public void TimeDisplayStringsGerman(double seconds, string result)
//        {
//            DateFormatter.DisplayTime(seconds, Translation.German).Should().Be(result);
//        }
//    }
//}