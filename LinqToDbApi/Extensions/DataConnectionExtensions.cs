using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.SqlQuery;
using LinqToDbApi.Models.Utils;

namespace LinqToDbApi.Extensions
{
    public static class DataConnectionExtensions
    {
        /// <summary>
        /// Check if table exists (using SQL code)
        /// </summary>
        public static bool TableExists(this DataConnection dataConnection, string? schemaName, string tableName)
        {
            Func<FullTableInfo, bool> condition;
            var catalogName = dataConnection.Connection.Database;
            if (string.IsNullOrEmpty(schemaName))
                condition = fti => fti.Catalog == catalogName 
                                   && fti.Schema == schemaName 
                                   && fti.Table == tableName;
            else
                condition = fti => fti.Catalog == catalogName && fti.Table == tableName;
            
            var query = dataConnection.GetTable<FullTableInfo>()
                .Where(condition).ToList();

            return query.Any();
        }

        /// <summary>
        /// Check if table exists (using table Type)
        /// </summary>
        public static bool TableExists(this DataConnection dataConnection, Type tableType)
        {
            if (!tableType.IsTable())
                return false;
            
            tableType.GetSchemaAndName(out var schema, out var name);
            return dataConnection.TableExists(schema, name);
        }

        /// <summary>
        /// Check if table exists (using ITable<T>)
        /// </summary>
        public static bool TableExists<T>(this DataConnection dataConnection, ITable<T> table) where T : class
        {
            return dataConnection.TableExists(table.SchemaName, table.TableName);
        }
        
        /// <summary>
        /// Returns Dictionry of <Type, bool> pairs, which tables are exists or not
        /// </summary>
        public static Dictionary<Type, bool> TablesExist(this DataConnection dataConnection,
            params Type[] tableTypes)
        {
            if(tableTypes.Length == 0)
                throw new Exception("Data collection is empty!");
            
            var tableInfos = tableTypes
                .Where(t=> t.IsTable())
                .ToDictionary(
                    ti => TableInfo.Build(dataConnection.Connection.Database, ti), 
                    t => t);
            
            var dict = new Dictionary<Type, bool>();
            try
            {
                var tempTable = dataConnection.CreateTempTable<TableInfo>(true);
                tempTable.BulkCopy(tableInfos.Keys);
                    //dataConnection.CreateTempTable(null, tableInfos.Keys);

                var query = (
                    from t in dataConnection.GetTable<FullTableInfo>()
                    from tempInfo in tempTable.Where(info => t.Schema == info.Schema && t.Table == info.Table)
                    select TableInfo.Build(tempInfo)
                    ).ToList();

                foreach (var (tableInfo, type) in tableInfos)
                    dict.Add(type, query.Any(ti=> ti.IsThis(tableInfo)));
            }
            finally
            {
                dataConnection.DropTable<TableInfo>();   
            }
            
            return dict;
        }
        
        /// <summary>
        /// Returns Dictionry of <Type, bool> pairs, which tables are exists or not
        /// </summary>
        public static Dictionary<Type, bool> TablesExist(this DataConnection dataConnection,
            IEnumerable<Type> tableTypes)
        {
            return dataConnection.TablesExist(tableTypes.ToArray());
        }

        /// <summary>
        /// Generate tables, that are not exist (by Type) 
        /// </summary>
        public static void CreateNotExistedTables(this DataConnection dataConnection, IEnumerable<Type> tableTypes)
        {
            if(!tableTypes.Any())
                return;
            
            var createArguments = dataConnection.GetCreateArguments();
            var createTableMemberInfo = typeof(DataExtensions).GetMethod("CreateTable");
            foreach (var type in tableTypes)
            {
                var refCreateTable = createTableMemberInfo.MakeGenericMethod(type);
                refCreateTable.Invoke(null, createArguments);
            }
        }

        /// <summary>
        /// Returns Dictionry of <Type, string> pairs, which columns are exists or not 
        /// </summary>
        public static IEnumerable<TableTypeColumnPair> CheckExistsTableColumnsName(this DataConnection dataConnection, IEnumerable<Type> tableTypes)
        {
            if(!tableTypes.Any())
                return new TableTypeColumnPair[0];

            var returnPairs = new List<TableTypeColumnPair>();

            try
            {
                var catalogName = dataConnection.Connection.Database;
                var columnInfos = tableTypes.Select(t => ColumnInfo.Build(catalogName, t)).ToArray();
                
                var tempTable = dataConnection.CreateTempTable<ColumnInfo>(true);
                var sequence = columnInfos.SelectMany(t => t).ToArray();
                tempTable.BulkCopy(sequence);

                var query = (
                    from ci in tempTable
                    from fci in dataConnection.GetTable<FullColumnInfo>().Where(
                        fullInfo => ci.Catalog == fullInfo.Catalog &&
                                    ci.Schema == fullInfo.Schema &&
                                    ci.Table == fullInfo.Table &&
                                    ci.Column == fullInfo.Column)
                    select ColumnInfo.Build(fci)).ToArray();
                var notExistingElements = sequence.Except(query, new ColumnInfoEqualityComparer());

                if (notExistingElements.Any())
                {
                    var tableTypeMap = tableTypes.ToDictionary(
                        t => TableInfo.Build(catalogName, t),
                        t => t);

                    var tableNotExistingElementsMap = notExistingElements.ToDictionary(
                        TableInfo.Build,
                        ci => ci.Column);

                    foreach (var (tableInfo, columnName) in tableNotExistingElementsMap)
                    {
                        if(string.IsNullOrEmpty(columnName))
                            continue;
                        
                        var type = tableTypeMap.First(map => map.Key.IsThis(tableInfo)).Value;
                        returnPairs.Add(new TableTypeColumnPair(type, columnName));
                    }
                }
            }
            finally
            {
                dataConnection.DropTable<ColumnInfo>();
            }
            
            return returnPairs;
        }

        /// <summary>
        /// Returns arguments, used for table creation (imported args extended with defaults)
        /// </summary>
        public static object[] GetCreateArguments(this DataConnection dataConnection, params object[] args)
        {
            var returnArgs = args.ToList();
            while (returnArgs.Count < 5)
                returnArgs.Add(null);
            if(returnArgs.Count < 6 )
                returnArgs.Add(DefaultNullable.None);
            if(returnArgs.Count< 7 )
                returnArgs.Add(null);
            returnArgs.Insert(0, dataConnection);
            return returnArgs.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public static TempTable<T> CreateTempTable<T>(this DataConnection dataConnection, bool dropOnExists) where T : class
        {
            var isExists = dataConnection.TableExists(typeof(T));
            if (isExists)
            {
                if (dropOnExists)
                    dataConnection.DropTable<T>();
                else
                    return (TempTable<T>)dataConnection.GetTable<T>();
            }
            return dataConnection.CreateTempTable<T>();
        }
    }
}