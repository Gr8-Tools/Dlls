namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    /// <summary>
    /// Native Connection connectionString settings options
    /// </summary>
    public abstract class NativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public abstract NpgsqlConnectionTypes ConnectionType { get; }
        
        /// <summary>
        /// ServerAddress
        /// </summary>
        public string Server { get; set; } = "localhost";
        
        /// <summary>
        /// Port number
        /// </summary>
        public int Port { get; set; } = 5432;

        /// <summary>
        /// DataBase Name
        /// </summary>
        public readonly string DataBase;

        protected NativeConnectionStringSettingsOptions(string dataBase)
        {
            DataBase = dataBase;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public abstract string GetConnectionString { get; }
    }
}