using System;
using System.Xml.Serialization;

namespace Simulator.Internals
{
    [Serializable]
    public sealed class Coordinate
    {
        /// <summary>
        /// The coordinate's value
        /// </summary>
        [XmlAttribute]
        public double Value { get; set; }

        /// <summary>
        /// Indicates if the coordinate has a fixed value
        /// </summary>
        [XmlAttribute]
        public bool IsFixed { get; set; }

        public bool ShouldSerializeIsFixed()
        {
            return IsFixed;
        }

        public Coordinate(double val, bool isFixed = false)
        {
            Value = val;
            IsFixed = isFixed;
        }
        public Coordinate()
        {
            
        }
    }
}
