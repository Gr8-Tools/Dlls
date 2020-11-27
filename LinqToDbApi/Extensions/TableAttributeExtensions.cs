using System;
using System.Linq;
using LinqToDB.Mapping;

namespace LinqToDbApi.Extensions
{
    public static class TableAttributeExtensions
    {
        /// <summary>
        /// Check if type is Table
        /// </summary>
        public static bool IsTable(this Type type)
            => type.GetCustomAttributes(typeof(TableAttribute), false).Any();
        
        /// <summary>
        /// Returns schemaName and tableName of Table
        /// </summary>
        public static void GetSchemaAndName(this Type tableType, out string? schema, out string name)
        {
            schema = null;
            name = tableType.Name;
            
            var tableAttributes = tableType.GetCustomAttributes(typeof(TableAttribute), false);
            if (tableAttributes.Length == 0)
                return;
            
            var tableAttribute = (TableAttribute)tableAttributes[0];
            if (!string.IsNullOrEmpty(tableAttribute.Schema))
                schema = tableAttribute.Schema;
            if (!string.IsNullOrEmpty(tableAttribute.Name))
                name = tableAttribute.Name;
        }
    }
}