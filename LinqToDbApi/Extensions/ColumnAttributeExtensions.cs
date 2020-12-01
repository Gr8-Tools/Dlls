using System;
using System.Reflection;
using LinqToDB.Mapping;

namespace LinqToDbApi.Extensions
{
    public static class ColumnAttributeExtensions
    {
        /// <summary>
        /// Check if property is column 
        /// </summary>
        public static bool IsColumn(this PropertyInfo pi)
        {
            return Attribute.GetCustomAttribute(pi, typeof(ColumnAttribute)) != null;
        }

        /// <summary>
        /// Returns column's name (if it's a Column and Name is set, else - PropertyInfo.Name) 
        /// </summary>
        public static string GetColumnName(this PropertyInfo pi)
        {
            var columnAttribute = (ColumnAttribute?) Attribute.GetCustomAttribute(pi, typeof(ColumnAttribute));
            return columnAttribute?.Name ?? pi.Name;
        }
    }
}