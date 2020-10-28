namespace Zxcvbn
{
    /// <summary>
    /// A summary of the attack times for the password.
    /// </summary>
    internal class AttackTimes
    {
        /// <summary>
        /// Gets or sets the display version of the crack times.
        /// </summary>
        public CrackTimesDisplay CrackTimesDisplay { get; set; }

        /// <summary>
        /// Gets or sets the numerical version of the crack times.
        /// </summary>
        public CrackTimes CrackTimesSeconds { get; set; }

        /// <summary>
        /// Gets or sets the overall score.
        /// </summary>
        public int Score { get; set; }
    }
}
