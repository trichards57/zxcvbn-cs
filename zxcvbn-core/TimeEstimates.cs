using System;

namespace Zxcvbn
{
    /// <summary>
    /// Estimates how long a password will take to crack under various conditions.
    /// </summary>
    internal static class TimeEstimates
    {
        /// <summary>
        /// Calculates the estimated attack times.
        /// </summary>
        /// <param name="guesses">The number of guesses required.</param>
        /// <returns>A summary of the estimated attack times.</returns>
        public static AttackTimes EstimateAttackTimes(double guesses)
        {
            var crackTimesSeconds = new CrackTimes
            {
                OfflineFastHashing1e10PerSecond = guesses / (100.0 / 3600),
                OfflineSlowHashing1e4PerSecond = guesses / 10,
                OnlineNoThrottling10PerSecond = guesses / 1e4,
                OnlineThrottling100PerHour = guesses / 1e10,
            };
            var crackTimesDisplay = new CrackTimesDisplay
            {
                OfflineFastHashing1e10PerSecond = DisplayTime(guesses / (100.0 / 3600)),
                OfflineSlowHashing1e4PerSecond = DisplayTime(guesses / 10),
                OnlineNoThrottling10PerSecond = DisplayTime(guesses / 1e4),
                OnlineThrottling100PerHour = DisplayTime(guesses / 1e10),
            };

            return new AttackTimes
            {
                CrackTimesDisplay = crackTimesDisplay,
                CrackTimesSeconds = crackTimesSeconds,
                Score = GuessesToScore(guesses),
            };
        }

        private static string DisplayTime(double seconds)
        {
            const double minute = 60;
            const double hour = minute * 60;
            const double day = hour * 24;
            const double month = day * 31;
            const double year = month * 12;
            const double century = year * 100;

            int? displayNumber = null;
            string displayString;

            if (seconds < 1)
                return "less than a second";
            if (seconds < minute)
            {
                displayNumber = (int)Math.Round(seconds);
                displayString = $"{displayNumber} second";
            }
            else if (seconds < hour)
            {
                displayNumber = (int)Math.Round(seconds / minute);
                displayString = $"{displayNumber} minute";
            }
            else if (seconds < day)
            {
                displayNumber = (int)Math.Round(seconds / hour);
                displayString = $"{displayNumber} hour";
            }
            else if (seconds < month)
            {
                displayNumber = (int)Math.Round(seconds / day);
                displayString = $"{displayNumber} day";
            }
            else if (seconds < year)
            {
                displayNumber = (int)Math.Round(seconds / month);
                displayString = $"{displayNumber} month";
            }
            else if (seconds < century)
            {
                displayNumber = (int)Math.Round(seconds / year);
                displayString = $"{displayNumber} year";
            }
            else
            {
                displayString = "centuries";
            }

            if (displayNumber.HasValue && displayNumber != 1)
                displayString += "s";

            return displayString;
        }

        private static int GuessesToScore(double guesses)
        {
            const int delta = 5;
            if (guesses < 1e3 + delta)
                return 0;
            if (guesses < 1e6 + delta)
                return 1;
            if (guesses < 1e8 + delta)
                return 2;
            if (guesses < 1e10 + delta)
                return 3;
            return 4;
        }
    }
}
