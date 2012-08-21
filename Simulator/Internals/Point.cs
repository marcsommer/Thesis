using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Simulator.Internals
{
    /// <summary>
    /// Represents a geometrical point
    /// </summary>
    [Serializable]
    public sealed class Point
    {
        /// <summary>
        /// X-coordinate of the point
        /// </summary>
		[XmlElement("X")]
        public Coordinate X { get; set; }

        /// <summary>
        /// Y-coordinate of the point
        /// </summary>
		[XmlElement("Y")]
        public Coordinate Y { get; set; }

		//[XmlElement(ElementName="Load")]
		public Force Load { get; set; }
        /// <summary>
        /// Creates a new point
        /// </summary>
        /// <param name="x">The x-coordinate of the point</param>
        /// <param name="y">The y-coordinate of the point</param>
        public Point(double x, double y)
        {
            X = new Coordinate(x);
            Y = new Coordinate(y);
        }

        /// <summary>
        /// Default constructor, mainly for parser
        /// </summary>
        public Point()
        {
            X = new Coordinate();
            Y = new Coordinate();
        }

		/// <summary>
		/// Compares the two points by their coordinates
		/// </summary>
		/// <param name="point">Other point to compare</param>
		/// <returns><i>true</i> if two points are equal (differ by less than <i>epsilon</i> for floating point numbers), <i>false</i> otherwise</returns>
		public bool Equals(Point point)
		{
			const double epsilon = 0.0000001;

            return ((Math.Abs(X.Value - point.X.Value) < epsilon && Math.Abs(Y.Value - point.Y.Value) < epsilon));
		}
    }

	/// <summary>
	/// Equality comparer for Enumerable extensions
	/// </summary>
	public class PointComparer : IEqualityComparer<Point>
	{
		#region IEqualityComparer<Point> Members

		/// <summary>
		/// Compares the equality of two points by their coordinates (by calling Point.Equals(Point))
		/// </summary>
		/// <param name="x">First point</param>
		/// <param name="y">Second point</param>
		/// <returns><i>true</i> if two points are equal (differ by less than 0.001 for floating point numbers), <i>false</i> otherwise</returns>
		/// <seealso cref="Point.Equals(Point)"/>
		public bool Equals(Point x, Point y)
		{
			return x.Equals(y);
		}

		/// <summary>
		/// Returns the hash code for point
		/// </summary>
		/// <param name="obj">Point object</param>
		/// <returns>A hash code for given point</returns>
		public int GetHashCode(Point obj)
		{
			unchecked
			{
				var hash = 17;

				hash = hash * 23 + obj.X.Value.GetHashCode();
				hash = hash * 23 + obj.Y.Value.GetHashCode();

				return hash;	
			}
		}

		#endregion
	}

}
