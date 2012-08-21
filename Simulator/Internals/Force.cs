using System;
using System.Xml.Serialization;

namespace Simulator.Internals
{
    /// <summary>
    /// Describes external/internal forces
    /// </summary>
    [Serializable]
    public sealed class Force
    {
        /// <summary>
        /// X-coordinate of the force vector
        /// </summary>
		[XmlAttribute("X")]
        public double X { get; set; }

		/// <summary>
		/// Y-coordinate of the force vector
		/// </summary>
		[XmlAttribute("Y")]
		public double Y { get; set; }

    }
}
