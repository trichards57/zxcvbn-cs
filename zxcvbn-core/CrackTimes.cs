namespace Zxcvbn
{
    /// <summary>
    /// Represents the time needed to crack the password under different conditions.
    /// </summary>
    public class CrackTimes
    {
        /// <summary>
        /// Gets the time if offline hashing at 1e10 per second.
        /// </summary>
        public double OfflineFastHashing1e10PerSecond { get; internal set; }

        /// <summary>
        /// Gets the time if offline hashing at 1e4 per second.
        /// </summary>
        public double OfflineSlowHashing1e4PerSecond { get; internal set; }

        /// <summary>
        /// Gets the time if online attempting the password at 10 per second.
        /// </summary>
        public double OnlineNoThrottling10PerSecond { get; internal set; }

        /// <summary>
        /// Gets the time if online attempting the password at 100 per hour.
        /// </summary>
        public double OnlineThrottling100PerHour { get; internal set; }
    }
}
