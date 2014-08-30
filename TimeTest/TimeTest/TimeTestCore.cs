using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace Training.NDonchev.TimeTest
{
    /// <summary>
    /// This class makes the timing possible.
    /// </summary>
    public class TimeTestCore
    {
        // initializing log events
        public event EventHandler AppStart;
        public event EventHandler AppStop;

        // initializing MaxFileSize constant
        private const int MAX_FILE_SIZE = 102400;
        private const string TEMPORARY_FILENAME = "timetest-temp.txt";
        private int _writingsCounter = 0;

        private DateTime _startTime;
        private DateTime _currentTime;

        public TimeTestCore()
        {
            this.AppStart += StartMessager;
            this.AppStop += StopMessager;
        }

        /// <summary>
        /// This method starts the timing.
        /// </summary>
        public void StartTiming()
        {
            // printing initial information on the console
            Console.WriteLine("To stop the application press any key.\n");

            Console.WriteLine("Writing to file:");

            // getting the start time
            _startTime = DateTime.Now.AddSeconds(1);

            // application start log event
            this.OnAppStart();

            // initializing variables
            var writeTimer = new Timer(1000);
            _currentTime = DateTime.MinValue;

            // timer event every second
            writeTimer.Elapsed += (sender, args) => this.WriteToFileAndLog(ref _currentTime);

            // starting the timer
            writeTimer.Start();

            if (Console.ReadKey() != null)
            {
                this.StopTiming(writeTimer);
            }
        }

        /// <summary>
        /// This method stops the timing.
        /// </summary>
        private void StopTiming(Timer timer)
        {
            // application stop log event
            this.OnAppStop();

            // stop the timer
            timer.Stop();

            // renaming the file to it's final name
            string finalFileName = this.RenameFile(_startTime, _currentTime, TEMPORARY_FILENAME);

            // printing on the console the name of the output file
            Console.WriteLine("\nOutput file is " + finalFileName);
        }

        /// <summary>
        /// Writing a start message to the logger.
        /// </summary>
        public virtual void OnAppStart()
        {
            if (AppStart != null)
            {
                AppStart(this, EventArgs.Empty);
            }
        }

        public virtual void OnAppStop()
        {
            if (AppStop != null)
            {
                AppStop(this, EventArgs.Empty);
            }
        }

        public void StartMessager(object sender, EventArgs e)
        {
            Logger.Write("The TimeTest application has started", "StartAndStop");
        }

        public void StopMessager(object sender, EventArgs e)
        {
            Logger.Write("The TimeTest application has stopped", "StartAndStop");
        }

        /// <summary>
        /// Writes to a file the current time each second.
        /// </summary>
        private void WriteToFileAndLog(ref DateTime currentTime)
        {
            if (_writingsCounter % 10 == 0)
            {
                // initializing log entry
                var log = new LogEntry();
                log.Categories = new List<string> { "WhileAppRunning" };
                log.Message = "This message appears every 10 seconds";

                // writing a log
                Logger.Write(log);
            }

            using (var fileWriter = new StreamWriter(TEMPORARY_FILENAME, true))
            {
                // initializing FileInto instance to check the file size
                var timeFileInfo = new FileInfo(TEMPORARY_FILENAME);

                // if the file size is below the allowed maximum
                if (timeFileInfo.Length < MAX_FILE_SIZE)
                {
                    // getting the current time and writing it to the file
                    currentTime = DateTime.Now;
                    fileWriter.WriteLine("{0:HH:mm:ss}", currentTime);

                    // writing on the console the current line added to the file
                    Console.WriteLine("{0:HH:mm:ss}", currentTime);
                }
                else
                {
                    // if the maximal allowed size of the file is reached, inform the user
                    fileWriter.WriteLine("\nThe length of the file exceeds the maximal allowed: " + MAX_FILE_SIZE);
                    Console.WriteLine("\nThe length of the file exceeds the maximal allowed: " + MAX_FILE_SIZE);
                }
            }

            _writingsCounter++;
        }

        /// <summary>
        /// Renames the temporary file name to a permanent name.
        /// </summary>
        private string RenameFile(DateTime startTime, DateTime endTime, string temporaryFileName)
        {
            // constructing the final filename
            var finalFileName = string.Format("timetest-{0:HHmmss}-{1:HHmmss}.txt", startTime, endTime);

            // renaming the file to the final filename
            File.Move(temporaryFileName, finalFileName);

            return finalFileName;
        }

        /// <summary>
        /// Logs a message on a specific period of time.
        /// </summary>
        private void LogTime()
        {
            if (_writingsCounter % 10 == 0)
            {
                // initializing log entry
                var log = new LogEntry();
                log.Categories = new List<string> { "WhileAppRunning" };
                log.Message = "This message appears every 10 seconds";

                // writing a log
                Logger.Write(log);
            }
        }
    }
}