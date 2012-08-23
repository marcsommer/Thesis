using System;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Simulator.Internals;
using Miscellaneous;

namespace Simulator.ViewModels
{
	public class SimulationViewModel : PropertyChangedBase
	{

		public SimulatorWrapper SimulatorWrapper
		{
			get { return simulatorWrapper; }
		}

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
		private readonly SimulatorWrapper simulatorWrapper;
		private readonly IFileDialogWrapper fileDialogWrapper;

		public SimulationViewModel(IWindowManager windowManager, IEventAggregator eventAggregator, IFileDialogWrapper fileDialog, SimulatorWrapper wrapper)
		{
			this.windowManager = windowManager;
			this.eventAggregator = eventAggregator;
			simulatorWrapper = wrapper;
			fileDialogWrapper = fileDialog;
		}

		public void Generate()
		{
			dynamic dialogSettings = new ExpandoObject();
			dialogSettings.Title = "Generate input";

			var viewModel = new InputGenerationViewModel();

			bool? result = windowManager.ShowDialog(viewModel, settings: dialogSettings);

			if(result == true)
			{
				var coefficients = new Coefficients
					{
						E = viewModel.ElasticityCoefficient,
						Gamma = viewModel.GammaAngle,
						Nu = viewModel.PoissonCoefficient,
						P = viewModel.Pressure
					};
				var config = Configuration.GenerateConfiguration(viewModel.InnerRadius, viewModel.OuterRadius,
				                                                 (int) viewModel.FiniteElementCount, coefficients);
				viewModel.Filename = viewModel.Filename ?? ("Input_" + DateTime.Now.ToShortDateString());
				SimulatorWrapper.ConfigurationIo.GenerateInputFile(config, viewModel.Filename);

				PrepareSimulation(new ConfigurationArguments(viewModel.Filename, config));
			}
		}

		public void OpenFile()
		{
			var result = fileDialogWrapper.ShowDialog();

			if(result == true)
			{
				PrepareSimulation(fileDialogWrapper.GetSelectedFileName());
			}
		}

		public void PrepareSimulation(ConfigurationArguments arguments)
		{
			IsBusy = true;
			Task.Factory.StartNew(() => SimulatorWrapper.PrepareSimulation(arguments)).ContinueWith(taskResult =>
				{
					IsBusy = false;
					NotifyOfPropertyChange(() => CanRunSimulation);
					if (taskResult.Exception != null)
						throw taskResult.Exception;
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public void RunSimulation()
		{
			IsBusy = true;
			Task.Factory.StartNew(() => SimulatorWrapper.Simulation.Run()).ContinueWith(taskResult =>
				{
					IsBusy = false;
					if (taskResult.Exception != null)
						throw taskResult.Exception;
					eventAggregator.Publish(new Events.SimulationFinishedEvent(taskResult.Result));
				}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		
		public bool CanRunSimulation
		{
			get
			{
				var nodeArray = SimulatorWrapper.NullSafe(x => x.Simulation).NullSafe(x => x.GetNodeList());
				return nodeArray != null && nodeArray.Any();
			}
		}

		public bool CanGenerate
		{
			get { return !CanRunSimulation; }
		}
	}
}
