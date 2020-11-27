using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDbApi.Connection.Utils;
using LinqToDbApi.Extensions;
using LinqToDbApi.Models.Utils;

namespace LinqToDbApi.Connection
{
    public abstract class BaseDataConnection: DataConnection, IAutoGenerate
    {
        public static readonly List<BaseDataConnection> Instances = new List<BaseDataConnection>();

        public event Action<Dictionary<Type, bool>> onExistedMapGet = dict => { }; 
        
        public event Action<IEnumerable<TableTypeColumnPair>> onExistedTablesStructureNeedUpdate = pairs => { };
        
        /// <summary>
        /// Enumerable of ITable Types
        /// </summary>
        public abstract IEnumerable<Type> SelfInitialize { get; }
        
        /// <summary>
        /// Connect to DB from default connection
        /// WARNING! Don't forget to dispose
        /// </summary>
        protected BaseDataConnection()
        {
            Instances.Add(this);
        }

        /// <summary>
        /// Connect to DB with read configuration from app.config (with ConfigurationManager)  
        /// WARNING! Don't forget to dispose
        /// </summary>
        protected BaseDataConnection(ConnectionStringSettings? configString)
            : base(configString?.ProviderName ?? "", configString?.ConnectionString ?? "")
        {
            Instances.Add(this);
        }

        /// <summary>
        /// Connect to DB with with   
        /// WARNING! Don't forget to dispose
        /// </summary>
        protected BaseDataConnection(IConnectionStringSettings connectionString)
            : base(connectionString.ProviderName ?? "", connectionString.ConnectionString)
        {
            Instances.Add(this);
        }
        
        /// <summary>
        /// Connect to DB with read configuration from app.config (with ConfigurationManager)  
        /// WARNING! Don't forget to dispose
        /// </summary>
        protected BaseDataConnection(string provider, string connectionString) : base(provider, connectionString)
        {
            Instances.Add(this);
        }

        /// <summary>
        /// Configuring connection
        /// </summary>
        public abstract void Configure();
        
        /// <summary>
        /// Function of generating tables
        /// </summary>
        public void AutoGenerate()
        {
            var existenceMap = this.TablesExist(SelfInitialize);
            onExistedMapGet(existenceMap);
            
            var existedTypes = existenceMap.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
            var notExistedTypes = existenceMap.Where(kvp => !kvp.Value).Select(kvp => kvp.Key).ToArray();
            
            this.CreateNotExistedTables(notExistedTypes);
            var needUpdatePairs = this.CheckExistsTableColumnsName(existedTypes).ToArray();
            if (needUpdatePairs.Any())
                onExistedTablesStructureNeedUpdate(needUpdatePairs);
        }
    }
}