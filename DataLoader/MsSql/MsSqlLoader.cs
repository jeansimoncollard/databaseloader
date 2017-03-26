using DatabaseLoader.Entities;
using DatabaseLoader.Properties;
using DatabaseLoader.Shared;
using Oracle.ManagedDataAccess.Client;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLoader.MsSql
{
    public class MsSqlLoader
    {
        private DataloadReader _dataloadReader;
        private ProcessStarter _processStarter;
        public MsSqlLoader()
        {
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
            var fileContent = File.ReadAllText(filePath);
            executeQuery(connectionString, _dataloadReader.GetInsertQuery(fileContent));

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
            executeQuery(connectionString, _dataloadReader.GetDeleteQuery(fileContent));
        }
        private void startUnloadProcess(string connectionString, string filePath)
        {
            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.GenerateExecutable = true;
            parameters.GenerateInMemory = false;
            parameters.OutputAssembly = $"uniquedataunloader{Guid.NewGuid()}.exe";

            _processStarter.StartProcess(connectionString, filePath, Resources.UnloadProcessMsSqlSourceCode, parameters);
        }

        private void executeQuery(string connectionString, string query)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"BEGIN TRANSACTION; {query} COMMIT TRANSACTION;";
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                handleQueryException(e, query);
            }
        }
        private void handleQueryException(SqlException e, string query)
        {
            throw new DatabaseLoaderException($"Line <{e.LineNumber}> caused the following error: <{e.Message}>", e, e.LineNumber);
        }


    }
}
