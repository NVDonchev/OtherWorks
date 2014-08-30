using System.Configuration;
using Training.NDonchev.TimeTest.Configuration;

namespace Training.NDonchev.TimeTest
{
    /// <summary>
    /// This class is used to process the destination of the logger from the config file,
    /// enable the required components and start the timing.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Starting the TimeTest application.
        /// </summary>
        public void RunApplication()
        {
            // instantiating
            var initializer = new Initializer();

            // getting logger destination
            var loggerConfigSection = ConfigurationManager.GetSection("loggerDestination") as loggerDestination;
            var dbImplementationConfigSection = ConfigurationManager.GetSection("dbImplementation") as dbImplementation;

            var timeTest = new TimeTestCore();

            if (loggerConfigSection.LogToTxt)
            {
                // enable listener
                initializer.EnableFileLogListener();
            }

            if (loggerConfigSection.LogToDb)
            {
                // enable listener
                initializer.EnableDatabaseLogListener();

                // initializing database
                initializer.InitDatabase();
            }

            // initialize logger
            initializer.InitLogger();

            // start timing
            timeTest.StartTiming();

            if (loggerConfigSection.LogToCsv)
            {
                // use DatabaseExporter to write to csv file
                var exporter = new DatabaseExporter();

                // choose how to export to csv file
                if (dbImplementationConfigSection.UseEntityFramework)
                {
                    exporter.ExportToCsvEntityFramework();
                }
                else
                {
                    exporter.ExportToCsvDataAccess();
                }
            }
        }
    }
}