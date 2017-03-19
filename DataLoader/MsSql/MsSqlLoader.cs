using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLoader.MsSql
{
    public class MsSqlLoader
    {
        /// <summary>
        /// This function tries to bulk load the data first. If it fails, it tries the conventional load to retrieve the error information.
        /// </summary>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded in the database.</param>
        /// <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        public void Load(string connectionString, string filePath, bool isPermanent)
        {

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

        }
    }
}
