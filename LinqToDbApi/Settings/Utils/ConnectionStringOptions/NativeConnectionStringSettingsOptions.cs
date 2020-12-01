namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    /// <summary>
    /// Native Connection connectionString settings options
    /// </summary>
    public abstract class NativeConnectionStringSettingsOptions
    {
        public const string DEFAULT_SERVER_NAME = "localhost";

        public const int DEFAULT_SERVER_PORT = 5432;
        
        /// <summary>
        /// ConnectionType
        /// </summary>
        public abstract NpgsqlConnectionTypes ConnectionType { get; }
        
        /// <summary>
        /// ServerAddress
        /// </summary>
        public string Server { get; set; } = DEFAULT_SERVER_NAME;
        
        /// <summary>
        /// Port number
        /// </summary>
        public int Port { get; set; } = DEFAULT_SERVER_PORT;

        /// <summary>
        /// DataBase Name
        /// </summary>
        public readonly string DataBase;

        protected NativeConnectionStringSettingsOptions(string dataBase)
        {
            DataBase = dataBase;
        }

        public NativeConnectionStringSettingsOptions Update(string? server, int? port)
        {
            Server = server ?? DEFAULT_SERVER_NAME;
            Port = port ?? DEFAULT_SERVER_PORT;
            return this;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public abstract string GetConnectionString { get; }
    }
}