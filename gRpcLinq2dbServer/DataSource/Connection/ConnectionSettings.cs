using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using LinqToDbApi.Settings;
using LinqToDbApi.Settings.Utils;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace gRpcLinq2dbServer.DataSource.Connection
{
    public class ConnectionSettings: NativeConnectionSettings
    {
        public override IEnumerable<IConnectionStringSettings> ConnectionStrings { get; }

        public ConnectionSettings(params NativeConnectionStringSettingsOptions[] optionsArray)
        {
            ConnectionStrings = optionsArray
                .Select(options => new NativeConnectionStringSettings(options))
                .ToArray();
        }
    }
}