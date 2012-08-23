using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miscellaneous
{
	public class Events
	{
		public class SimulationFinishedEvent
		{
			public double[] Result { get; private set; }
			
			public SimulationFinishedEvent(double[] result)
			{
				Result = result;
			}
		}
	}
}
