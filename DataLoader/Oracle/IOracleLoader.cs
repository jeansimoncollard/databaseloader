using DatabaseLoader.Shared;

namespace DatabaseLoader.Oracle
{

    /// <summary>
    /// Provides methods to load and unload in the databse the ".dataload" files. It is implemented by OracleLoader.
    /// </summary>
    public interface IOracleLoader : ILoader
    {
        /// <summary>
        /// This function defers all deferable constraints before loading the data in the database. This makes the loading much faster, but loses all error information if the loading fails. 
        /// </summary>
        /// <remarks>
        /// The database must contain deferable constraints for BulkLoad to make a difference.
        /// </remarks>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded.</param>
        /// <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        void BulkLoad(string connectionString, string filePath, bool isPermanent);

        /// <summary>
        /// This function loads the data in the database using simple insert statements.
        /// </summary>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded.</param>
        ///  <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        void ConventionalLoad(string connectionString, string filePath, bool isPermanent);

        /// <summary>
        /// This function tries to bulk load the data first. If it fails, it tries the conventional load to retrieve the error information.
        /// </summary>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded in the database.</param>
        /// <param name="isPermanent">Whether to leave the data in the database after the process is finished running. True to not make the data stay in the database, false to automatically unload it when process terminates.</param>
        void Load(string connectionString, string filePath, bool isPermanent);

        /// <summary>
        /// This function unloads the data from the database.
        /// </summary>
        /// <remarks>
        /// If the data set contains relative dates, the data must be unloaded on the same day that it has been loaded.
        /// </remarks>
        /// <param name="connectionString">Connection string of the database to load the data into.</param>
        /// <param name="filePath">Full path of the ".dataload" file to be loaded.</param>
        void Unload(string connectionString, string filePath);
    }
}