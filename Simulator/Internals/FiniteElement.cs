using System;
using System.Xml.Serialization;

namespace Simulator.Internals
{
    /// <summary>
    /// Represents a finite element (a triangle) with three vertices
    /// </summary>
    [Serializable]
    public sealed class FiniteElement
    {
        /// <summary>
        /// First vertex of the triangle (counter-clockwise)
        /// </summary>
		[XmlElement("PointA")]
        public Point A { get; set; }

        /// <summary>
        /// Second vertex of the triangle (counter-clockwise)
        /// </summary>
		[XmlElement("PointB")]
        public Point B { get; set; }

        /// <summary>
        /// Third vertex of the triangle (counter-clockwise)
        /// </summary>
		[XmlElement("PointC")]
        public Point C { get; set; }

    }
}
