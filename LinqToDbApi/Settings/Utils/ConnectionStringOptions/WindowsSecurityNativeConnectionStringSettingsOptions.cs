namespace LinqToDbApi.Settings.Utils.ConnectionStringOptions
{
    public class WindowsSecurityNativeConnectionStringSettingsOptions : NativeConnectionStringSettingsOptions
    {
        /// <summary>
        /// ConnectionType
        /// </summary>
        public override NpgsqlConnectionTypes ConnectionType => NpgsqlConnectionTypes.WindowsSecurity;

        public WindowsSecurityNativeConnectionStringSettingsOptions(string dataBase) : base(dataBase) { }

        /// <summary>
        /// Returns connection string from set properties
        /// </summary>
        public override string GetConnectionString
            => $"Server={Server};Port={Port};Database={DataBase};Integrated Security=true;";
    }
}