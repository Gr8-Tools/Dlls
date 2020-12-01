namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    public class CommandTimeoutNativeConnectionStringSettingsOptions: StandardNativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.CommandTimeout;
        
        /// <summary>
        /// The CommandTimeout parameter is measured in seconds and controls for how long to wait for a command to finish before throwing an error
        /// </summary>
        public int CommandTimeout { get; }
        
        public CommandTimeoutNativeConnectionStringSettingsOptions(string dataBase, string userId, string password, int commandTimeout) : base(dataBase, userId, password)
        {
            CommandTimeout = commandTimeout;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString 
            => $"{base.GetConnectionString}CommandTimeout={CommandTimeout};";
    }
}