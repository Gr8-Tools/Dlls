namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    public class ControllingPoolNativeConnectionStringSettingsOptions: ProtocolNativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.ControllingPool;

        public int MinPoolSize { get; }

        public int MaxPoolSize { get; }

        public int ConnectionLifeTime { get; } = -1;
        
        public ControllingPoolNativeConnectionStringSettingsOptions(string dataBase, string userId, string password, int minPoolSize, int maxPoolSize, int protocolVersion = 3) 
            : base(dataBase, userId, password, protocolVersion)
        {
            MinPoolSize = minPoolSize;
            MaxPoolSize = maxPoolSize;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString
        {
            get
            {
                var value = $"{base.GetConnectionString}Pooling=true;MinPoolSize={MinPoolSize};MaxPoolSize={MaxPoolSize};";
                if (ConnectionLifeTime != -1)
                    value += $"ConnectionLifeTime={ConnectionLifeTime}";

                return value;
            }
        } 
    }
}