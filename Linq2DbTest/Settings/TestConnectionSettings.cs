using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using LinqToDbApi.Settings;
using LinqToDbApi.Settings.Utils;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace Linq2DbTest.Settings
{
    public class TestConnectionSettings: NativeConnectionSettings
    {
        public override IEnumerable<IConnectionStringSettings> ConnectionStrings { get; }

        public TestConnectionSettings(params NativeConnectionStringSettingsOptions[] optionsArray)
        {
            ConnectionStrings = optionsArray
                .Select(options => new NativeConnectionStringSettings(options))
                .ToArray();
        }
    }
}