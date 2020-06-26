using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Sql
{
    public class SqlTypeResolver : TypeResolverBase, ITypeResolver
    {
        public override string DefaultCollectionFormat { get; set; } = "{0}";

        protected override string ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            if (typeInfo == null)
            {
                return null;
            }

            if (typeInfo.HasStereotype("SQL Type Override"))
            {
                return typeInfo.GetStereotypeProperty<string>("SQL Type Override", "Type Name");
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
                    result = "BYTE";
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
                    result = $"DECIMAL({typeInfo.GetStereotypeProperty("Numeric", "Precision", "18")}, {typeInfo.GetStereotypeProperty("Numeric", "Scale", "2")})";
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
                    result = $"{typeInfo.GetStereotypeProperty("Text", "SQL Data Type", "NVARCHAR")}({typeInfo.GetStereotypeProperty("Text", "Max Length", "MAX")})";
                    break;
            }

            result += $" {(typeInfo.IsNullable ? "NULL" : "NOT NULL")}";

            return result;
        }
    }
}