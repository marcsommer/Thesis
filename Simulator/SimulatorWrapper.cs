using Simulator.Internals;
using Simulator.Internals.Interfaces;

namespace Simulator
{
	public class SimulatorWrapper
	{
		public ISimulation Simulation { get; private set; }
		public IConfigurationIo ConfigurationIo { get; private set; }

		public SimulatorWrapper(ISimulation simulation, IConfigurationIo configurationIo)
		{
			Simulation = simulation;
			ConfigurationIo = configurationIo;
		}

		public void PrepareSimulation(ConfigurationArguments arguments)
		{
			Simulation.LoadConfiguration(arguments.Configuration ?? ConfigurationIo.ReadInput(arguments.ConfigurationFilename));
		}
	}

	public class ConfigurationArguments
	{
		public string ConfigurationFilename { get; private set; }
		public Configuration Configuration { get; private set; }

		public ConfigurationArguments(string filename, Configuration config)
        {
            ConfigurationFilename = filename;
            Configuration = config;
        }

        public static implicit operator ConfigurationArguments(string filename)
        {
            return new ConfigurationArguments(filename, null);
        }
	}
}
