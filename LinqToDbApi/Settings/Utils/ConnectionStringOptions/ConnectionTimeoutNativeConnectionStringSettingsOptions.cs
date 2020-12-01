namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    public class ConnectionTimeoutNativeConnectionStringSettingsOptions: StandardNativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.ConnectionTimeout;
        
        /// <summary>
        /// The Timeout parameter is measured in seconds and controls for how long to wait for a connection to open before throwing an error
        /// </summary>
        public int Timeout { get; }
        
        public ConnectionTimeoutNativeConnectionStringSettingsOptions(string dataBase, string userId, string password, int timeout) : base(dataBase, userId, password)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString 
            => $"{base.GetConnectionString}Timeout={Timeout};";
    }
}