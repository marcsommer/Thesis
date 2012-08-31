using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;

namespace ResultDisplay.ViewModels
{
	public class ResultDisplayViewModel : PropertyChangedBase
	{
		private IEnumerable<Point> originalPoints;
		private IEnumerable<Point> movedPoints;

		public IEnumerable<Point> OriginalPoints
		{
			get { return originalPoints; }
			set
			{
				if (Equals(value, originalPoints)) return;
				originalPoints = value;
				NotifyOfPropertyChange(() => OriginalPoints);
			}
		}

		public IEnumerable<Point> MovedPoints
		{
			get { return movedPoints; }
			set
			{
				if (Equals(value, movedPoints)) return;
				movedPoints = value;
				NotifyOfPropertyChange(() => MovedPoints);
			}
		}
	}
}
