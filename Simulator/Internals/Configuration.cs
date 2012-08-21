using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace Simulator.Internals
{
    /// <summary>
    /// Configuration class with data for the simulation
    /// </summary>
    [XmlRoot("Configuration")]
    public sealed class Configuration
    {
        /// <summary>
        /// All coefficients
        /// </summary>
        public Coefficients Coefficients { get; set; }

        /// <summary>
        /// List of the finite elements
        /// </summary>
        public List<FiniteElement> Elements { get; set;}

		public static Configuration GenerateConfiguration(double innerRadius, double outerRadius, int triangleCount, Coefficients coefficients)
		{
            if (triangleCount < 6)
                throw new ArgumentException("Number of triangles has to be more than 6. Currently: " + triangleCount);
            var innerPoints = new List<Point>();
            var outerPoints = new List<Point>();
            var elementCount = triangleCount / 2;
            int outerCount = (elementCount - 1) / 2;
            int innerCount = outerCount - 1;

            outerPoints.Add(new Point { X = new Coordinate(outerRadius, true), Y = new Coordinate(0, true) });
            innerPoints.Add(new Point { X = new Coordinate(innerRadius, true), Y = new Coordinate(0, true) });

            for (var i = 1; i <= outerCount; ++i)
            {

                var argOuter = (2 * 90 * i * Math.PI) / (360 * (outerCount + 1));
                var outerPoint = new Point
	                {
		                X =
			                {
				                Value = Math.Cos(argOuter)*outerRadius
			                },
		                Y =
			                {
				                Value = Math.Sin(argOuter)*outerRadius
			                },
		                Load = new Force
			                {
				                X = coefficients.P*Math.Sin(coefficients.Gamma*i/elementCount),
				                Y = coefficients.P*Math.Cos(coefficients.Gamma*i/elementCount)
			                }
	                };

	            outerPoints.Add(outerPoint);


                if (i == outerCount) continue;

                var point = new Point();
                var argInner = (2 * 90 * i * Math.PI) / (360 * (innerCount + 1));
                point.X.Value = Math.Cos(argInner) * innerRadius;
                point.Y.Value = Math.Sin(argInner) * innerRadius;

                innerPoints.Add(point);
            }
            outerPoints.Add(new Point { X = new Coordinate(0), Y = new Coordinate(outerRadius) });
            innerPoints.Add(new Point { X = new Coordinate(0), Y = new Coordinate(innerRadius) });
            var elementList = new List<FiniteElement>();

            for (var i = 0; i <= elementCount / 2; i++)
            {

                var element = new FiniteElement
	                {
		                A = innerPoints[i],
		                B = outerPoints[i],
		                C = outerPoints[i + 1]
	                };

	            elementList.Add(element);
                if (i + 1 >= innerPoints.Count)
                    break;

                var secondElement = new FiniteElement
	                {
		                A = innerPoints[i],
		                B = outerPoints[i + 1],
		                C = innerPoints[i + 1]
	                };

	            elementList.Add(secondElement);
            }

            var secondList = elementList.Select(element =>
            {
                var mirrorElement = Extensions.DeepCopy(element);
                mirrorElement.A.X.Value = -mirrorElement.A.X.Value;
                mirrorElement.B.X.Value = -mirrorElement.B.X.Value;
                mirrorElement.C.X.Value = -mirrorElement.C.X.Value;

                return mirrorElement;
            }).ToList();
            var list = new List<FiniteElement>();
            list.AddRange(elementList);
            list.AddRange(secondList);
            var config = new Configuration { Elements = list, Coefficients = coefficients };
            return config;
		}
    }
}
