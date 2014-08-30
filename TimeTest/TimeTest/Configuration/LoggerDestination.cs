using System.Configuration;

namespace Training.NDonchev.TimeTest.Configuration
{
    /// <summary>
    /// Adds a loggerDestination section in the app.config file.
    /// </summary>
    public class loggerDestination : ConfigurationSection
    {
        /// <summary>
        /// Adds a property logToTxt in the config section.
        /// </summary>
        [ConfigurationProperty("logToTxt", DefaultValue = true, IsRequired = false)]
        public bool LogToTxt
        {
            get
            {
                return (bool)this["logToTxt"];
            }
        }

        /// <summary>
        /// Adds a property logToDb in the config section.
        /// </summary>
        [ConfigurationProperty("logToDb", DefaultValue = false, IsRequired = false)]
        public bool LogToDb
        {
            get
            {
                return (bool)this["logToDb"];
            }
        }

        /// <summary>
        /// Adds a property logToCsv in the config section.
        /// </summary>
        [ConfigurationProperty("logToCsv", DefaultValue = false, IsRequired = false)]
        public bool LogToCsv
        {
            get
            {
                return (bool)this["logToCsv"];
            }
        }
    }
}