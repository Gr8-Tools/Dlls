using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;
using LinqToDbApi.Extensions;

namespace LinqToDbApi.Models.Utils
{
    [Table(Schema = "information_schema", Name = "columns_cut")]
    public class ColumnInfo
    {
        [Column(Name = "table_catalog")]
        public string Catalog { get; set; }
        
        [Column(Name = "table_schema")]
        public string Schema { get; set; }
        
        [Column(Name = "table_name")]
        public string Table { get; set; }
        
        [Column(Name = "column_name")]
        public string? Column { get; set; }

        public bool IsThis(object? obj)
        {
            switch (obj)
            {
                case ColumnInfo columnInfo:
                    return columnInfo.Catalog == Catalog
                           && columnInfo.Schema == Schema
                           && columnInfo.Table == Table
                           && (columnInfo.Column ?? "") == (Column ?? "");
            }

            return false;
        }
        
        public override string ToString()
            => $"[Catalog]:{Catalog};[Schema]:{Schema};[Table]:{Table};[Column]:{Column}";

        /// <summary>
        /// Build Enumerable of ColumnInfo bu DataBaseName and TableType for each property 
        /// </summary>
        public static IEnumerable<ColumnInfo> Build(string catalogue, Type tableType)
        {
            if(!tableType.IsTable())
                return new ColumnInfo[0];

            tableType.GetSchemaAndName(out var schema, out var table);
            
            return tableType.GetProperties()
                .Select(pi => pi.GetColumnName())
                .Select(columnName => new ColumnInfo
                {
                    Catalog = catalogue,
                    Schema = schema ?? "",
                    Table = table,
                    Column = columnName
                });
        }
        
        /// <summary>
        /// Build cut version of ColumnInfo from FullColumnInfo 
        /// </summary>
        public static ColumnInfo Build(FullColumnInfo fci)
        {
            return new ColumnInfo
            {
                Catalog = fci.Catalog,
                Schema = fci.Schema,
                Table = fci.Table,
                Column = fci.Column
            };
        }
    }

    public class TableTypeColumnPair
    {
        public readonly Type TableType;
        public readonly string ColumnName;

        public TableTypeColumnPair(Type tableType, string columnName)
        {
            TableType = tableType;
            ColumnName = columnName;
        }
    }

    [Table(Schema = "information_schema", Name = "columns")]
    public class FullColumnInfo
    {
        [Column(Name = "table_catalog")]
        public string Catalog { get; set; }
        
        [Column(Name = "table_schema")]
        public string Schema { get; set; }
        
        [Column(Name = "table_name")]
        public string Table { get; set; }
        
        [Column(Name = "column_name")]
        public string Column { get; set; }

        [Column(Name = "ordinal_position")] 
        public int OrdinalPosition { get; set; }
        
        [Column(Name = "column_default")]
        public string ColumnDefault { get; set; }
        
        [Column(Name = "is_nullable")]
        public string IsNullable { get; set; }
        
        [Column(Name = "data_type")]
        public string DataType { get; set; }
        
        [Column(Name = "character_maximum_length")]
        public int MaxLength { get; set; }
        
        [Column(Name = "character_octet_length")]
        public int OctetLength { get; set; }
        
        [Column(Name = "numeric_precision")]
        public int NumericPrecision { get; set; }
        
        [Column(Name = "numeric_precision_radix")]
        public int NumericPrecisionRadix { get; set; }
        
        [Column(Name = "numeric_scale")]
        public int NumericScale { get; set; }
        
        [Column(Name = "datetime_precision")]
        public int DatetimePrecision { get; set; }
        
        [Column(Name = "interval_type")]
        public string IntervalType { get; set; }
        
        [Column(Name = "interval_precision")]
        public int IntervalPrecision { get; set; }
        
        [Column(Name = "character_set_catalog")]
        public string CharacterSetCatalog { get; set; }
        
        [Column(Name = "character_set_schema")]
        public string CharacterSetSchema { get; set; }
        
        [Column(Name = "character_set_name")]
        public string CharacterSetName { get; set; }
        
        [Column(Name = "collation_catalog")]
        public string CollationCatalog { get; set; }
        
        [Column(Name = "collation_schema")]
        public string CollationSchema { get; set; }
        
        [Column(Name = "collation_name")]
        public string CollationName { get; set; }
        
        [Column(Name = "domain_catalog")]
        public string DomainCatalog { get; set; }
        
        [Column(Name = "domain_schema")]
        public string DomainSchema { get; set; }
        
        [Column(Name = "domain_name")]
        public string DomainName { get; set; }
        
        [Column(Name = "udt_catalog")]
        public string UdtCatalog { get; set; }
        
        [Column(Name = "udt_schema")]
        public string UdtSchema { get; set; }
        
        [Column(Name = "udt_name")]
        public string UdtName { get; set; }
        
        [Column(Name = "scope_catalog")]
        public string ScopeCatalog { get; set; }
        
        [Column(Name = "scope_schema")]
        public string ScopeSchema { get; set; }
        
        [Column(Name = "scope_name")]
        public string ScopeName { get; set; }
        
        [Column(Name = "maximum_cardinality")]
        public int MaxCardinality { get; set; }
        
        [Column(Name = "dtd_identifier")]
        public string DtdIdentifier { get; set; }
        
        [Column(Name = "is_self_referencing")]
        public string IsSelfReferencing { get; set; }
        
        [Column(Name = "is_identity")]
        public string IsIdentity { get; set; }
        
        [Column(Name = "identity_generation")]
        public string IdentityGeneration { get; set; }
        
        [Column(Name = "identity_start")]
        public string IdentityStart { get; set; }
        
        [Column(Name = "identity_increment")]
        public string IdentityIncrement { get; set; }
        
        [Column(Name = "identity_maximum")]
        public string IdentityMax { get; set; }
        
        [Column(Name = "identity_minimum")]
        public string IdentityMin { get; set; }
        
        [Column(Name = "identity_cycle")]
        public string IdentityCycle { get; set; }
        
        [Column(Name = "is_generated")]
        public string IsGenerated { get; set; }
        
        [Column(Name = "generation_expression")]
        public string GeneratedExpression { get; set; }
        
        [Column(Name = "is_updatable")]
        public string IsUpdatable { get; set; }
    }

    public class ColumnInfoEqualityComparer : IEqualityComparer<ColumnInfo>
    {
        public bool Equals(ColumnInfo x, ColumnInfo y)
        {
            if (x == null) 
                throw new ArgumentNullException(nameof(x));
            if (y == null) 
                throw new ArgumentNullException(nameof(y));
            return x.IsThis(y);
        }

        public int GetHashCode(ColumnInfo obj)
        {
            return HashCode.Combine(obj.Catalog, obj.Schema, obj.Table, obj.Column);
        }
    } 
}