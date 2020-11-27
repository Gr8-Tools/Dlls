using System.Configuration;
using System.Linq;

namespace LinqToDbApi.Settings
{
    /// <summary>
    /// Read connection settings with ConfigurationManager
    /// </summary>
    public static class ConnectionConfigSettings
    {
        /// <summary>
        /// Collection of connection settings
        /// </summary>
        public static ConnectionStringSettingsCollection ConnectionInfos
        {
            get
            {
                if (_connectionInfos == null)
                    ReadConnectionsStrings();
                
                return _connectionInfos;
            }
            private set => _connectionInfos = value;
        }

        private static ConnectionStringSettingsCollection? _connectionInfos;
        
        /// <summary>
        /// Reserve ConnectionStringSettingsCollection from ConfigurationManager
        /// </summary>
        public static void ReadConnectionsStrings()
        {
            ConnectionInfos = ConfigurationManager.ConnectionStrings;
        }
    }

    public static class ConfigurationConnectionSettingsExtensions
    {
        /// <summary>
        /// Returns ConnectionStringSettings with set name
        /// </summary>
        public static ConnectionStringSettings? GetConnectionStringSettings(
            this ConnectionStringSettingsCollection connectionInfos, string name)
            => (from ConnectionStringSettings connectionInfo in connectionInfos
                where connectionInfo.Name == name
                select connectionInfo).FirstOrDefault();
        
        /// <summary>
        /// Returns connectionString with set name
        /// </summary>
        public static string? GetConnectionString(this ConnectionStringSettingsCollection connectionInfos, string name)
            => (from ConnectionStringSettings connectionInfo 
                    in connectionInfos 
                where connectionInfo.Name == name 
                select connectionInfo.ConnectionString).FirstOrDefault();
        
        /// <summary>
        /// Returns providerValue with set name
        /// </summary>
        public static string? GetProvider(this ConnectionStringSettingsCollection connectionInfos, string name)
            => (from ConnectionStringSettings connectionInfo 
                    in connectionInfos 
                where connectionInfo.Name == name 
                select connectionInfo.ProviderName).FirstOrDefault();
    }
}