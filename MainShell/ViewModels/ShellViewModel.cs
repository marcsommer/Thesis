using Caliburn.Micro;
using Miscellaneous;
using ResultDisplay.ViewModels;
using Simulator.ViewModels;

namespace MainShell.ViewModels
{
	public sealed class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<Events.SimulationFinishedEvent>
	{
		public TabViewModel<SimulationViewModel> SimulationTab { get; private set; }
		public TabViewModel<ResultDisplayViewModel> ResultTab { get; private set; } 

		public ShellViewModel(SimulationViewModel simulationViewModel, ResultDisplayViewModel resultDisplayViewModel, IEventAggregator eventAggregator)
		{
			SimulationTab = new TabViewModel<SimulationViewModel>("Simulation", simulationViewModel);
			ResultTab = new TabViewModel<ResultDisplayViewModel>("Results", resultDisplayViewModel);
			eventAggregator.Subscribe(this);
			ActivateItem(SimulationTab);
		}
		
		public void Handle(Events.SimulationFinishedEvent message)
		{
			ResultTab.ContentViewModel.MovedPoints = message.MovedPoints;
			ResultTab.ContentViewModel.OriginalPoints = message.OriginalPoints;
			ActivateItem(ResultTab);
		}
	}
}
