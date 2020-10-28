namespace Zxcvbn.Matcher
{
    /// <summary>
    /// A local implementation of Point to avoid referring to the graphical libraries.
    /// </summary>
    internal struct Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the x coordinate of the point.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the y coordinate of the point.
        /// </summary>
        public int Y { get; }
    }
}
