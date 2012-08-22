using Simulator.ViewModels;

namespace MainShell.ViewModels
{
	public class ShellViewModel : IShell
	{
		public SimulationViewModel SimulationViewModel { get; private set; }

		public ShellViewModel(SimulationViewModel viewModel)
		{
			SimulationViewModel = viewModel;
		}
	}
}
