using System;
using LinqToDB.Mapping;
using LinqToDbApi.Extensions;

namespace LinqToDbApi.Models.Utils
{
    [Table(Schema = "information_schema", Name = "tables_cut")]
    public class TableInfo
    {
        [Column(Name = "table_catalog")]
        public string Catalog { get; set; }
        
        [Column(Name = "table_schema")] 
        public string? Schema { get; set; }
        
        [Column(Name = "table_name"), NotNull] 
        public string Table { get; set; }
        
        public bool IsThis(object? obj)
        {
            switch (obj)
            {
                case TableInfo tableInfo:
                    return tableInfo.Catalog == Catalog 
                           && tableInfo.Schema == Schema 
                           && tableInfo.Table == Table;
            }

            return false;
        }

        public static TableInfo Build(ColumnInfo columnInfo)
        {
            return new TableInfo
            {
                Catalog = columnInfo.Catalog,
                Schema = columnInfo.Schema,
                Table = columnInfo.Table
            };
        }
        
        public static TableInfo Build(string catalog, Type tableType)
        {
            tableType.GetSchemaAndName(out var schema, out var table);
            return new TableInfo
            {
                Catalog = catalog,
                Schema = schema,
                Table = table
            };
        }

        public static TableInfo Build(TableInfo ti)
        {
            return new TableInfo
            {
                Catalog = ti.Catalog,
                Schema = ti.Schema,
                Table = ti.Table
            };
        }

        public static TableInfo Build(FullTableInfo fti)
        {
            return new TableInfo
            {
                Catalog = fti.Catalog,
                Schema = fti.Schema,
                Table = fti.Table
            };
        }
    }

    [Table(Schema = "information_schema", Name = "tables")]
    public class FullTableInfo
    {
        [Column(Name = "table_catalog")]
        public string Catalog { get; set; }
        
        [Column(Name = "table_schema")]
        public string Schema { get; set; }
        
        [Column(Name = "table_name")]
        public string Table { get; set; }
        
        [Column(Name = "table_type")]
        public string Type { get; set; }
        
        [Column(Name = "self_referencing_column_name")]
        public string ColumnName { get; set; }
        
        [Column(Name = "reference_generation")]
        public string ReferenceGeneration { get; set; }
        
        [Column(Name = "user_defined_type_catalog")]
        public string UserDefinedTypeCatalog { get; set; }
        
        [Column(Name = "user_defined_type_schema")]
        public string UserDefinedTypeSchema { get; set; }
        
        [Column(Name = "user_defined_type_name")]
        public string UserDefinedTypeName { get; set; }
        
        [Column(Name = "is_insertable_into")]
        public string IsInterectableInto { get; set; }
        
        [Column(Name = "is_typed")]
        public string IsTyped { get; set; }
        
        [Column(Name = "commit_action")]
        public string CommitAction { get; set; }
    }
}