namespace DatabaseLoader.MsSql
{
    /// <summary>
    /// Provides methods to load and unload in the database the ".dataload" files. It is implemented by MsSqlLoader.
    /// </summary>
    public interface IMsSqlLoader
    {

        /// <summary>
        /// This loads the compiled file from Database Loader Editor
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