using System.Collections.Generic;
using System.Windows;

namespace Miscellaneous
{
	public class Events
	{
		public class SimulationFinishedEvent
		{
			public IEnumerable<Point> OriginalPoints { get; private set; }
			public IEnumerable<Point> MovedPoints { get; private set; } 
			
			public SimulationFinishedEvent(IEnumerable<Point> originalPoints, IEnumerable<Point> movedPoints)
			{
				OriginalPoints = originalPoints;
				MovedPoints = movedPoints;
			}
		}
	}
}
