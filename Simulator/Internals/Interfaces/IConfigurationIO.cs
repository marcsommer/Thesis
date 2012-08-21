namespace Simulator.Internals.Interfaces
{
    /// <summary>
    /// Input file access interface
    /// </summary>
    public interface IConfigurationIo
    {
        /// <summary>
        /// Creates the Configuration class based on the file
        /// </summary>
        /// <param name="filename">Input filename</param>
        /// <returns>A Configuration created from the file</returns>
        /// <seealso cref="Configuration"/>
        Configuration ReadInput(string filename);

        /// <summary>
        /// Creates a configuration file from existing Configuration class instance
        /// </summary>
        /// <param name="config">Configuration to be serialized to XML</param>
        /// <param name="filename">Desired input filename</param>
        void GenerateInputFile(Configuration config, string filename);
    }
}