using System.Configuration;

namespace Training.NDonchev.TimeTest.Configuration
{
    /// <summary>
    /// Adds a dbImplementation section in the app.config file.
    /// </summary>
    public class dbImplementation : ConfigurationSection
    {
        /// <summary>
        /// Adds a property useEntityFramework in the config section.
        /// </summary>
        [ConfigurationProperty("useEntityFramework", DefaultValue = false, IsRequired = false)]
        public bool UseEntityFramework
        {
            get
            {
                return (bool)this["useEntityFramework"];
            }
        }
    }
}
