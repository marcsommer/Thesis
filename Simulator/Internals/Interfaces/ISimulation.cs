namespace Simulator.Internals.Interfaces
{
    /// <summary>
    /// Publicly exposed interface to run the simulation
    /// </summary>
    public interface ISimulation
    {
        /// <summary>
        /// Runs the simulation
        /// </summary>
        double[] Run();

        void LoadConfiguration(Configuration configuration);

        Point[] GetNodeList();

        int ElementCount();
    }
}