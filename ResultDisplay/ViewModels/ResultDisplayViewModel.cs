using Caliburn.Micro;

namespace ResultDisplay.ViewModels
{
	public class ResultDisplayViewModel : PropertyChangedBase
	{
		private double[] result;
		public double[] Result
		{
			get { return result; }
			set
			{
				if (Equals(value, result)) return;
				result = value;
				NotifyOfPropertyChange(() => Result);
			}
		}
	}
}
