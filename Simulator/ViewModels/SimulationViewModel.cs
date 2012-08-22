using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Simulator.ViewModels
{
	public class SimulationViewModel : PropertyChangedBase
	{
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				if (value.Equals(isBusy)) return;
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
			}
		}

		private readonly IWindowManager windowManager;
		private readonly IEventAggregator eventAggregator;
		private bool isBusy;

		public SimulationViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
		{
			this.windowManager = windowManager;
			this.eventAggregator = eventAggregator;
		}


		public void Generate()
		{
			dynamic dialogSettings = new ExpandoObject();
			dialogSettings.Title = "Generate input";

			var viewModel = new InputGenerationViewModel();

			bool? result = windowManager.ShowDialog(viewModel, settings: dialogSettings);

			if(result == true)
			{
				//Use the above view model to generate the input XML
			}
		}

		public void RunSimulation()
		{
			IsBusy = true;
			Task.Factory.StartNew(() => Thread.Sleep(5000)).ContinueWith(result =>
				{
					IsBusy = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
