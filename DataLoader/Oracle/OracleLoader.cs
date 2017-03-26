using DatabaseLoader.Entities;
using DatabaseLoader.Properties;
using DatabaseLoader.Shared;
using Microsoft.CSharp;
using Oracle.ManagedDataAccess.Client;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseLoader.Oracle
{
    /// <summary>
    /// This class is responsible for loading and unloading from the database the ".dataload" files created with Database Loader Editor.
    /// </summary>
    public class OracleLoader : IOracleLoader
    {
        private CSharpCodeProvider _cSharpCodeProvider;
        private DataloadReader _dataloadReader;
        private ProcessStarter _processStarter;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public OracleLoader()
        {
            _cSharpCodeProvider = new CSharpCodeProvider();
            _dataloadReader = new DataloadReader();
            _processStarter = new ProcessStarter();
        }


        /// <summary>
        /// This function tries to bulk load the data first. If it fails, it tries the conventional load to retrieve the error information.
        /// </summary>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded in the database.</param>
        /// <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        public void Load(string connectionString, string filePath, bool isPermanent)
        {
            try
            {
                bulkLoadPermanent(connectionString, filePath);
            }
            catch (Exception e1)
            {
                try
                {
                    conventionalLoadPermanent(connectionString, filePath); //If loading fails, do the conventional load that will fail too, but give more precise error info.
                }
                catch (Exception e2)
                {
                    throw new DatabaseLoaderException(e2.Message, e1);
                }
            }
            if (!isPermanent)
            {
                startUnloadProcess(connectionString, filePath);
            }
        }


        /// <summary>
        /// This function loads the data in the database using simple insert statements.
        /// </summary>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded.</param>
        ///  <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        public void ConventionalLoad(string connectionString, string filePath, bool isPermanent)
        {

            conventionalLoadPermanent(connectionString, filePath);
            if (!isPermanent)
            {
                startUnloadProcess(connectionString, filePath);
            }
        }


        /// <summary>
        /// This function defers all deferable constraints before loading the data in the database. This makes the loading much faster, but loses all error information if the loading fails. 
        /// </summary>
        /// <remarks>
        /// The database must contain deferable constraints for BulkLoad to make a difference.
        /// </remarks>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded.</param>
        /// <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        public void BulkLoad(string connectionString, string filePath, bool isPermanent)
        {
            bulkLoadPermanent(connectionString, filePath);
            if (!isPermanent)
            {
                startUnloadProcess(connectionString, filePath);
            }
        }



        /// <summary>
        /// This function unloads the data from the database.
        /// </summary>
        /// <remarks>
        /// If the data set contains relative dates, the data must be unloaded on the same day that it has been loaded.
        /// </remarks>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded.</param>
        public void Unload(string connectionString, string filePath)
        {
            var fileContent = File.ReadAllText(filePath);

            var query = _dataloadReader.GetDeleteQuery(fileContent);
            try
            {
                executeDeleteOrInsertQueryDeferConstraints(connectionString, query);
            }
            catch (Exception e1)
            {
                try
                {
                    executeDeleteOrInsertQuery(connectionString, query); //If loading fails, do the conventional unload that will fail too, but give more precise error info.
                }
                catch (Exception e2)
                {
                    throw new DatabaseLoaderException(e2.Message, e1);
                }
            }
        }

        private void bulkLoadPermanent(string connectionString, string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            executeQuery(connectionString, _dataloadReader.GetInsertQuery(fileContent), executeDeleteOrInsertQueryDeferConstraints);
        }

        private void conventionalLoadPermanent(string connectionString, string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            executeQuery(connectionString, _dataloadReader.GetInsertQuery(fileContent), executeDeleteOrInsertQuery);
        }

        private void executeQuery(string connectionString, string query, Action<string, string> executeQuery)
        {
            try
            {
                executeQuery(connectionString, query);
            }
            catch (OracleException e)
            {
                handleQueryException(e, query);
            }
        }



        private void executeDeleteOrInsertQuery(string connectionString, string query)
        {
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    var command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandText = $"begin {query} end;";
                    command.ExecuteNonQuery();
                }
            }
            catch (OracleException e)
            {
                handleQueryException(e, query);
            }
        }

        private void handleQueryException(OracleException e, string query)
        {
            var lineposition = e.Message.IndexOf(" line ");
            if (lineposition != -1) //Find on what line the error was
            {
                var i = 0;
                while (lineposition + 6 + i < e.Message.Length && char.IsDigit(e.Message[lineposition + 6 + i]))
                {
                    i++;
                }
                var lineNumber = Convert.ToInt32(e.Message.Substring(lineposition + 6, i));

                var dividedQueryLines = divideQueryLines(query);

                throw new DatabaseLoaderException($"Line <{dividedQueryLines[lineNumber - 1].Value}> caused the following error: <{e.Message}>", e, Convert.ToInt32(dividedQueryLines[lineNumber - 1].Value));
            }
            throw e;
        }

        private void executeDeleteOrInsertQueryDeferConstraints(string connectionString, string query)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var command = new OracleCommand();
                command.Connection = connection;
                command.CommandText = $"begin execute immediate 'SET CONSTRAINTS ALL DEFERRED'; {query} end;";
                command.ExecuteNonQuery();
            }
        }

        private void startUnloadProcess(string connectionString, string filePath)
        {          
            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("Oracle.ManagedDataAccess.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.GenerateExecutable = true;
            parameters.GenerateInMemory = false;
            parameters.OutputAssembly = $"uniquedataunloader{Guid.NewGuid()}.exe";         

            _processStarter.StartProcess(connectionString, filePath, Resources.UnloadProcessOracleSourceCode, parameters);
        }



        private MatchCollection divideQueryLines(string program)
        {
            var textToAnalyze = program;
            Regex regex = new Regex(@"^.*$", RegexOptions.Multiline);
            return regex.Matches(textToAnalyze);
        }

    }
}
