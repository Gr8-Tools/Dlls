namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    public class ProtocolNativeConnectionStringSettingsOptions: StandardNativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.ProtocolVersion;
        
        /// <summary>
        /// Valid values for the key Protocol is 2 or 3.
        /// </summary>
        public int Protocol { get; }
        
        public ProtocolNativeConnectionStringSettingsOptions(string dataBase, string userId, string password, int protocolVersion = 3) : base(dataBase, userId, password)
        {
            Protocol = protocolVersion;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString 
            => $"{base.GetConnectionString}Protocol={Protocol};";
    }
}