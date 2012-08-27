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
