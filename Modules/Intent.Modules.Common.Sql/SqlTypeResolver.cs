using System;
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

        public class SqlTypeResolverContext : TypeResolverContextBase
        {
            public SqlTypeResolverContext() : base(CollectionFormatter.Create("{0}"), TypeResolution.DefaultNullableFormatter.Instance)
            {
            }

            protected override IResolvedTypeInfo ResolveType(ITypeReference typeReference, INullableFormatter nullableFormatter)
            {
                if (typeReference == null)
                {
                    return null;
                }

                if (typeReference.HasStereotype("SQL Type Override"))
                {
                    return ResolvedTypeInfo.Create(
                        name: typeReference.GetStereotypeProperty<string>("SQL Type Override", "Type Name"),
                        isPrimitive: true,
                        typeReference: typeReference,
                        template: null,
                        nullableFormatter: null,
                        collectionFormatter: null);
                }

                string result = null;
                switch (typeReference.Element.Name)
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
                        result = $"DECIMAL({typeReference.GetStereotypeProperty("Decimal Constraints", "Precision", "18")}, {typeReference.GetStereotypeProperty("Decimal Constraints", "Scale", "2")})";
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
                    case "short":
                        result = "SMALLINT";
                        break;
                    case "string":
                        var type = typeReference.GetStereotypeProperty("Text Constraints", "SQL Data Type", "NVARCHAR").Replace("DEFAULT", "NVARCHAR");
                        result = $"{type}({typeReference.GetStereotypeProperty("Text Constraints", "MaxLength", typeReference.GetStereotypeProperty("Text Constraints", "Max Length", "MAX"))})";
                        break;
                }

                result += $" {(typeReference.IsNullable ? "NULL" : "NOT NULL")}";

                return ResolvedTypeInfo.Create(
                    name: result,
                    isPrimitive: true,
                    typeReference: typeReference,
                    template: null,
                    nullableFormatter: null,
                    collectionFormatter: null);
            }
        }
    }
}