namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    public class SslDisabledNativeConnectionStringSettingsOptions : ProtocolNativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.SslDisabled;

        public SslDisabledNativeConnectionStringSettingsOptions(string dataBase, string userId, string password,
            int protocolVersion = 3)
            : base(dataBase, userId, password, protocolVersion)
        {
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString
            => $"{base.GetConnectionString}SSL=false;SslMode=Disable;";
    }
}