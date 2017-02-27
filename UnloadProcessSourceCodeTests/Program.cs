using Oracle.ManagedDataAccess.Client;
using System;
using System.Diagnostics;
using System.IO;

namespace DatabaseLoaderStartupProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //Spaces in string are separated in different args, so first arg tells how many spaces are in file path.
            //second arg is parent id
            //third arg and following args is connection string (splitted because it contains spaces)
            //couple last arg is file path 
            var numberOfSpacesInFileName = Convert.ToInt32(args[0]);

            var parentProcessId = args[1];

            var filePath = "";

            for (var j = args.Length - 1; j >= args.Length - 1 - numberOfSpacesInFileName; j--)
            {
                filePath = args[j] + " " + filePath;
            }


            var connectionString = args[2];
            var i = 3;

            while (i < args.Length - 1 - numberOfSpacesInFileName)
            {
                connectionString += " " + args[i];
                i++;
            }

            var dataDeleter = new DataDeleter();
            var deleteQuery = dataDeleter.GetDeleteQuery(File.ReadAllText(filePath));

            var parent = Process.GetProcessById(Convert.ToInt32(parentProcessId));
            parent.WaitForExit();

            dataDeleter.ExecuteDeleteQuery(connectionString, deleteQuery);
        }
    }

    class DataDeleter
    {

        public string GetDeleteQuery(string fileContent)
        {
            var insertQueryLength = getLengthOfInsertQuery(fileContent);

            var startIndex = insertQueryLength.Length + Environment.NewLine.Length + Convert.ToInt32(insertQueryLength);

            return fileContent.Substring(startIndex, fileContent.Length - startIndex);
        }

        public void ExecuteDeleteQuery(string connectionString, string query)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand();
                command.Connection = connection;
                command.CommandText = "begin execute immediate 'SET CONSTRAINTS ALL DEFERRED'; " + query + " end;";
                command.ExecuteNonQuery();
            }
        }

        private string getLengthOfInsertQuery(string fileContent)
        {
            var insertQueryLength = "";
            var i = 0;
            while (char.IsDigit(fileContent[i]))
            {
                insertQueryLength += fileContent[i];
                i++;
            }
            return insertQueryLength;
        }
    }
}