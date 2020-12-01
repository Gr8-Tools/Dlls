using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;
using LinqToDbApi.Settings.Extensions;
using LinqToDbApi.Settings.Utils;
using LinqToDbApi.Settings.Utils.ConnectionStringOptions;

namespace LinqToDbApi.Settings
{
    public abstract class NativeConnectionSettings: ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();
        public string DefaultConfiguration => "PostgreSQL";
        public string DefaultDataProvider => "PostgreSQL";

        private IEnumerable<NativeConnectionStringSettings> NativeConnectionStringSettings =>
            ConnectionStrings.OfType<NativeConnectionStringSettings>();
        
        public abstract IEnumerable<IConnectionStringSettings> ConnectionStrings { get; }

        public IConnectionStringSettings? Get<T>(T enumValue) where T : Enum
        {
            if (enumValue is NpgsqlConnectionTypes npgsqlType) 
                return NativeConnectionStringSettings.FirstOrDefault(ncs => ncs.ConnectionType == npgsqlType);

            return null;
        }

        public static IEnumerable<NativeConnectionStringSettingsOptions> GenerateAllOptions(
            NativeConnectionStringSettingsOptionsData optionsData)
        {
            var optionList = new List<NativeConnectionStringSettingsOptions>();

            if (!optionsData.WindowsSecuritySupported)
                return optionList;
            
            optionList.Add(new WindowsSecurityNativeConnectionStringSettingsOptions(optionsData.DataBase));

            if (!optionsData.UserExists)
                return optionList.UpdateOptions(optionsData.Server, optionsData.Port);
            
            optionList.Add(new StandardNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password));
            
            if(optionsData.CommandTimeoutSupported)
                optionList.Add(new CommandTimeoutNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password, optionsData.CommandTimeout ?? 0));
            if (optionsData.ConnectionTimeoutSupported)
                optionList.Add(new ConnectionTimeoutNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password, optionsData.ConnectionTimeout ?? 0));
            if (optionsData.ProtocolConnectionsSupported)
            {
                optionList.AddRange(new []
                {
                    new ProtocolNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password, optionsData.Protocol ?? 0),
                    new SslActiveNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password, optionsData.Protocol ?? 0),
                    new SslDisabledNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password, optionsData.Protocol ?? 0),
                });
                
                if(optionsData.PoolControllingSupported)
                    optionList.Add(new ControllingPoolNativeConnectionStringSettingsOptions(optionsData.DataBase, optionsData.UserId, optionsData.Password,
                        optionsData.MinPoolSize ?? 0, optionsData.MaxPoolSize ?? 0, optionsData.Protocol ?? 0));
            }
            
            return optionList.UpdateOptions(optionsData.Server, optionsData.Port);
        } 
    }
}