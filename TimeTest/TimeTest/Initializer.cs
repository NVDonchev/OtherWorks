using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Database.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Training.NDonchev.TimeTest
{
    /// <summary>
    /// Initializer of components of the application.
    /// </summary>
    public class Initializer
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        public void InitDatabase()
        {
            // setting the data directory path
            var dataDirectoryPath = this.SetDatabaseDirPath();

            // reading the database script file
            var scriptStr = this.ReadDbScriptFile(dataDirectoryPath);

            // setting the path and filename for the logging database
            var databaseFilePath = string.Format(@"{0}\Database\Logging.mdf", dataDirectoryPath);

            // preparing to execute SQL statement for creating database
            var commandStr = string.Format("CREATE DATABASE Logging ON PRIMARY (NAME=Logging, FILENAME='{0}')", databaseFilePath);
            var dbForCreatingFile = new SqlDatabase(@"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True;");
            var command = dbForCreatingFile.GetSqlStringCommand(commandStr);
            var script = dbForCreatingFile.GetSqlStringCommand(scriptStr);

            // creating database file if not present and populating it via a script file
            if (!File.Exists(databaseFilePath))
            {
                dbForCreatingFile.ExecuteNonQuery(command);
                var dbForPopulatingFile = new SqlDatabase(@"Data Source=(LocalDB)\v11.0;AttachDBFilename=|DataDirectory|\Database\Logging.mdf;Integrated Security=True;");
                dbForPopulatingFile.ExecuteNonQuery(script);
            }

            // modifying app.config to include AttachDBFilename in the connection string as the logger needs it
            this.ModifyAppConfigFile();
        }

        /// <summary>
        /// Initializes the logger.
        /// </summary>
        public void InitLogger()
        {
            // creating DatabaseProviderFactory
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());

            // creating logger
            var logWriterFactory = new LogWriterFactory();
            Logger.SetLogWriter(logWriterFactory.Create());
        }

        /// <summary>
        /// Enables the database log listener.
        /// </summary>
        public void EnableDatabaseLogListener()
        {
            var xmlConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = (LoggingSettings)xmlConfiguration.GetSection(LoggingSettings.SectionName);

            var traceListener = setting.TraceListeners.Get("DatabaseTraceListener") as FormattedDatabaseTraceListenerData;
            traceListener.Filter = SourceLevels.All;

            xmlConfiguration.Save();
            ConfigurationManager.RefreshSection(LoggingSettings.SectionName);
        }

        /// <summary>
        /// Enables the file log listener.
        /// </summary>
        public void EnableFileLogListener()
        {
            var xmlConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = (LoggingSettings)xmlConfiguration.GetSection(LoggingSettings.SectionName);

            var startStopListener = setting.TraceListeners.Get("StartAndStop");
            startStopListener.Filter = SourceLevels.All;

            var whileRunningListener = setting.TraceListeners.Get("WhileAppRunning");
            whileRunningListener.Filter = SourceLevels.All;

            xmlConfiguration.Save();
            ConfigurationManager.RefreshSection(LoggingSettings.SectionName);
        }

        /// <summary>
        /// Reads the database script file.
        /// </summary>
        private string ReadDbScriptFile(string dataDirectoryPath)
        {
            // setting the path and filename for the database script
            var scriptPath = string.Format(@"{0}\LoggingDbScript\CreateLoggingDatabaseObjects.sql", dataDirectoryPath);
            var scriptFile = new FileInfo(scriptPath);

            // reading the script file
            var scriptStr = string.Empty;
            using (var reader = scriptFile.OpenText())
            {
                scriptStr = reader.ReadToEnd();
            }

            return scriptStr;
        }

        /// <summary>
        /// Modifying the app.config file to include AttachDBFilename in the database connection string.
        /// </summary>
        private void ModifyAppConfigFile()
        {
            // modifying app.config to include AttachDBFilename in the connection string
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            var newConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDBFilename=|DataDirectory|\Database\Logging.mdf;Integrated Security=True;";

            connectionStringsSection.ConnectionStrings["Logging"].ConnectionString = newConnectionString;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        /// <summary>
        /// Sets the data directory.
        /// </summary>
        private string SetDatabaseDirPath()
        {
            // setting relative path to database file
            var dataDirectoryPath = Environment.CurrentDirectory.Replace(@"\bin\Debug", string.Empty);
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectoryPath);

            // creating database folder if not present
            var databaseDirectoryPath = string.Format(@"{0}\Database", dataDirectoryPath);
            if (!Directory.Exists(databaseDirectoryPath))
            {
                Directory.CreateDirectory(databaseDirectoryPath);
            }

            return dataDirectoryPath;
        }
    }
}