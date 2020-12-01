using LinqToDB.Configuration;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace LinqToDbApi.Settings.Utils
{
    /// <summary>
    /// Класс сущности настроек соединения
    /// </summary>
    public class NativeConnectionStringSettings: IConnectionStringSettings
    {
        public NpgsqlConnectionTypes ConnectionType { get; } = NpgsqlConnectionTypes.Null;
        
        public string Name { get; set; } = "PostgreSQL";
        public string ProviderName { get; set; } = "Npgsql";
        public string ConnectionString { get; set; }
        public bool IsGlobal { get; }
        
        public NativeConnectionStringSettings(NativeConnectionStringSettingsOptions options, bool isGlobal = false)
        {
            ConnectionType = options.ConnectionType;
            IsGlobal = isGlobal;
            ConnectionString = options.GetConnectionString;
        }
        
        // public static NativeConnectionStringSettings DefaultSettings(bool isGlobal = false)
        // {
        //     return new NativeConnectionStringSettings(isGlobal)
        //     {
        //         Name = "PostgreSQL",
        //         ProviderName = "Npgsql",
        //         ConnectionString =
        //             "Server=localhost;Port=5432;Database=linq2dbTest;User Id=postgres;Password=A!s2D#f4;Pooling=true;MinPoolSize=10;MaxPoolSize=100;Protocol=3;"
        //     };
        // }
    }
}