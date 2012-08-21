using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Simulator.Internals.Interfaces.Implementations
{
    /// <summary>
    /// A file access interface implementation
    /// </summary>
    internal sealed class ConfigurationIo : IConfigurationIo
    {
        #region IFileAccess Members

        /// <summary>
        /// Creates the Configuration class based on the file
        /// </summary>
        /// <param name="filename">Input filename</param>
        /// <exception cref="ArgumentNullException">Thrown when the filename is null</exception>
        /// <returns>A Configuration created from the file</returns>
        /// <seealso cref="Configuration"/>
        public Configuration ReadInput(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");
			
			if (!filename.EndsWith(".xml"))
				filename += ".xml";

            Configuration config;

            var serializer = new XmlSerializer(typeof(Configuration));
			using (var reader = XmlReader.Create(new FileStream(filename, FileMode.Open)))
            {
                config = (Configuration) serializer.Deserialize(reader);
            }
            
            return config;
        }

    	/// <summary>
    	/// Creates a configuration file from existing Configuration class instance
    	/// </summary>
    	/// <param name="config">Configuration to be serialized to XML</param>
    	/// <param name="filename">Desired input filename</param>
    	public void GenerateInputFile(Configuration config, string filename)
        {
            var serializer = new XmlSerializer(typeof (Configuration));

    		filename = filename ?? string.Format("GeneratedInput_{0}.xml", DateTime.Now.ToShortDateString());
			if (!filename.EndsWith(".xml"))
				filename += ".xml";

            using (var writer = XmlWriter.Create(new FileStream(filename, FileMode.Create)))
            {
                serializer.Serialize(writer, config);
            }
        }
        #endregion
    }
}