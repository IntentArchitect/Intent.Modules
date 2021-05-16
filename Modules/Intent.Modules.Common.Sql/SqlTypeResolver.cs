using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Sql
{
    public class SqlTypeResolver : TypeResolverBase, ITypeResolver
    {
        public SqlTypeResolver() : base(new SqlTypeResolverContext())
        {
        }

        protected override ITypeResolverContext CreateContext()
        {
            return new SqlTypeResolverContext();
        }
    }
    public class SqlTypeResolverContext : TypeResolverContextBase
    {
        public SqlTypeResolverContext() : base(new CollectionFormatter("{0}"))
        {
        }

        protected override string FormatGenerics(IResolvedTypeInfo type, IEnumerable<IResolvedTypeInfo> genericTypes)
        {
            throw new NotImplementedException();
        }

        protected override ResolvedTypeInfo ResolveType(ITypeReference typeInfo)
        {
            if (typeInfo == null)
            {
                return null;
            }

            if (typeInfo.HasStereotype("SQL Type Override"))
            {
                return new ResolvedTypeInfo(typeInfo.GetStereotypeProperty<string>("SQL Type Override", "Type Name"), true, null);
            }

            string result = null;
            switch (typeInfo.Element.Name)
            {
                case "binary":
                    result = "VARBINARY";
                    break;
                case "bool":
                    result = "BIT";
                    break;
                case "byte":
                    result = "TINYINT";
                    break;
                case "char":
                    result = "CHAR";
                    break;
                case "date":
                    result = "DATE";
                    break;
                case "datetime":
                    result = "DATETIME";
                    break;
                case "datetimeoffset":
                    result = "DATETIMEOFFSET";
                    break;
                case "decimal":
                    result = $"DECIMAL({typeInfo.GetStereotypeProperty("Decimal Constraints", "Precision", "18")}, {typeInfo.GetStereotypeProperty("Decimal Constraints", "Scale", "2")})";
                    break;
                case "double":
                    result = "FLOAT";
                    break;
                case "float":
                    result = "FLOAT";
                    break;
                case "guid":
                    result = "UNIQUEIDENTIFIER";
                    break;
                case "int":
                    result = "INT";
                    break;
                case "long":
                    result = "BIGINT";
                    break;
                case "object":
                    throw new Exception("Cannot convert type 'object' to a valid SQL Data Type");
                    break;
                case "short":
                    result = "SMALLINT";
                    break;
                case "string":
                    var type = typeInfo.GetStereotypeProperty("Text Constraints", "SQL Data Type", "NVARCHAR").Replace("DEFAULT", "NVARCHAR");
                    result = $"{type}({typeInfo.GetStereotypeProperty("Text Constraints", "MaxLength", typeInfo.GetStereotypeProperty("Text Constraints", "Max Length", "MAX"))})";
                    break;
            }

            result += $" {(typeInfo.IsNullable ? "NULL" : "NOT NULL")}";

            return new ResolvedTypeInfo(result, true, null);
        }
    }
}