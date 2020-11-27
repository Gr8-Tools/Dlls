using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace LinqToDbApi.Settings
{
    public abstract class NativeConnectionSettings: ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();
        public string DefaultConfiguration => "PostgreSQL";
        public string DefaultDataProvider => "PostgreSQL";
        
        public abstract IEnumerable<IConnectionStringSettings> ConnectionStrings { get; }
    }
}