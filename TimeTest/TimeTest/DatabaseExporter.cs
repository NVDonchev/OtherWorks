using FileHelpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using TimeTest.Data;

namespace Training.NDonchev.TimeTest
{
    /// <summary>
    /// Exporter of database information to a csv file.
    /// </summary>
    public class DatabaseExporter
    {
        /// <summary>
        /// Exports logging information from the database, using Entity Framework to a csv file.
        /// </summary>
        public void ExportToCsvEntityFramework()
        {
            // creating folder for the output file
            CreateFileFolder();

            // instantiation of the FileHelper engine
            var engine = new FileHelperEngine(typeof(Log));

            // instantiation of the LoggingEntities context
            var dbContext = new LoggingEntities();

            // getting the rows of the db table
            var rows = dbContext.Logs;

            // writing to a csv file, using FileHelpers engine
            foreach (var row in rows)
            {
                engine.AppendToFile(@"..\..\CSV_Files\LoggingData.csv", row);
            }

            // printing message of success
            Console.WriteLine("\nExport logging information to CSV file, using Entity Framework successful.");
        }

        /// <summary>
        /// Exports logging information from the database to a csv file.
        /// </summary>
        public void ExportToCsvDataAccess()
        {
            // initializing
            StringBuilder sb = null;
            var csvRows = new List<string>();
            var command = "Select * From Log";
            var separator = ",";

            // creating default database
            var factory = new DatabaseProviderFactory();
            var db = factory.CreateDefault();

            // using dataReader
            using (IDataReader dataReader = db.ExecuteReader(CommandType.Text, command))
            {
                // while there is data to read
                while (dataReader.Read())
                {
                    var value = string.Empty;
                    sb = new StringBuilder();

                    // iterating through the fields
                    for (int index = 0; index < dataReader.FieldCount; index++)
                    {
                        // if current field is not null
                        if (!dataReader.IsDBNull(index))
                        {
                            // getting the value
                            value = dataReader.GetValue(index).ToString();

                            // if the data type of the field is string
                            if (dataReader.GetFieldType(index) == typeof(string))
                            {
                                // if double quotes are used in value, ensure each are replaced but 2
                                if (value.IndexOf("\"") >= 0)
                                {
                                    value = value.Replace("\"", "\"\"");
                                }

                                // if separtor are is in value, ensure it is put in double quotes
                                if (value.IndexOf(separator) >= 0)
                                {
                                    value = "\"" + value + "\"";
                                }
                            }

                            // if FormattedMessage column is current
                            if (value.Contains("Timestamp") && value.Contains("Message"))
                            {
                                // replacing the new line in the text of FormattedMessage column with a separator
                                value = this.FixFormattedMessageColumn(value);
                            }

                            // appending the value to a StringBuilder
                            sb.Append(value);

                            // if it's not the last column in the row and the value is not null or empty
                            if (index < dataReader.FieldCount - 1 && !string.IsNullOrEmpty(value))
                            {
                                // appending the separator
                                sb.Append(separator);
                            }
                        }
                    }

                    // adding current row to a list
                    csvRows.Add(sb.ToString());
                }
            }

            // writing to a CSV file
            this.WriteToFile(csvRows);

            // printing message of success
            Console.WriteLine("\nExport logging information to CSV file successful.");
        }

        /// <summary>
        /// Fixes an issue in the FormattedMessageColumn - removes inappropriate line break in the text
        /// and places a separator instead.
        /// </summary>
        private string FixFormattedMessageColumn(string value)
        {
            // finding the index of the line break symbol
            int index = value.IndexOf("Message") - 1;

            // removing line break
            var removedLineBreak = value.Remove(index, 1);

            // inserting separator
            var fixedValue = removedLineBreak.Insert(index, " | ");

            return fixedValue;
        }

        /// <summary>
        /// Writes to a csv file.
        /// </summary>
        /// <param name="csvRows"></param>
        private void WriteToFile(List<string> csvRows)
        {
            var dirPath = CreateFileFolder();

            // instatiating a StreamWriter
            var sw = new StreamWriter(dirPath + @"\LoggingData.csv");
            using (sw)
            {
                // writing to the file for each row
                foreach (var row in csvRows)
                {
                    sw.WriteLine(row);
                }
            }
        }

        private string CreateFileFolder()
        {
            // getting the path to the application and appending a folder for the CSV files
            var dirPath = Environment.CurrentDirectory.Replace(@"\bin\Debug", string.Empty);
            dirPath += @"\CSV_Files";

            // creating a folder if such doesn't exist
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            return dirPath;
        }
    }
}