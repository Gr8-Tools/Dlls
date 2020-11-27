namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    /// <summary>
    /// Native Connection connectionString settings options
    /// </summary>
    public class StandardNativeConnectionStringSettingsOptions: NativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.Standard;

        /// <summary>
        /// User Name
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Password
        /// </summary>
        protected string _password;
        
        public StandardNativeConnectionStringSettingsOptions(string dataBase, string userId, string password) : base(dataBase)
        {
            UserId = userId;
            _password = password;
        }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString
            => $"Server={Server};Port={Port};Database={DataBase};User Id={UserId};Password={_password};";
    }
}