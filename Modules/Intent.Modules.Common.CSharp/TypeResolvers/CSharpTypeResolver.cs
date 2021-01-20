using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    public class CSharpTypeResolver : TypeResolverBase, ITypeResolver
    {
        private readonly ICSharpProject _project;

        public CSharpTypeResolver(ICSharpProject project)
        {
            _project = project;
        }

        public override string DefaultCollectionFormat { get; set; } = "IEnumerable<{0}>";

        protected override IResolvedTypeInfo ResolveType(ITypeReference typeInfo, string collectionFormat = null)
        {
            var result = typeInfo.Element.Name;
            var isPrimitive = true;
            if (typeInfo.Element.Stereotypes.Any(x => x.Name == "C#"))
            {
                string typeName = typeInfo.Element.GetStereotypeProperty<string>("C#", "Type", typeInfo.Element.Name);
                string @namespace = typeInfo.Element.GetStereotypeProperty<string>("C#", "Namespace");
                isPrimitive = typeInfo.Element.GetStereotypeProperty("C#", "Is Primitive", true);
                result = !string.IsNullOrWhiteSpace(@namespace) ? $"{@namespace}.{typeName}" : typeName;

                if (typeInfo.IsNullable && isPrimitive)
                {
                    result += "?";
                }
            }
            else
            {
                switch (typeInfo.Element.Name)
                {
                    case "bool":
                        result = "bool";
                        break;
                    case "date":
                    case "datetime":
                        result = "System.DateTime";
                        break;
                    case "char":
                        result = "char";
                        break;
                    case "byte":
                        result = "byte";
                        break;
                    case "decimal":
                        result = "decimal";
                        break;
                    case "double":
                        result = "double";
                        break;
                    case "float":
                        result = "float";
                        break;
                    case "short":
                        result = "short";
                        break;
                    case "int":
                        result = "int";
                        break;
                    case "long":
                        result = "long";
                        break;
                    case "datetimeoffset":
                        result = "System.DateTimeOffset";
                        break;
                    case "binary":
                        result = "byte[]";
                        isPrimitive = false;
                        break;
                    case "object":
                        result = "object";
                        break;
                    case "guid":
                        result = "System.Guid";
                        break;
                    case "string":
                        result = "string";
                        isPrimitive = false;
                        break;
                }

                if (typeInfo.IsNullable && (isPrimitive || _project.IsNullableAwareContext() || typeInfo.Element.SpecializationType.Equals("Enum", StringComparison.InvariantCultureIgnoreCase)))
                {
                    result += "?";
                }
            }

            if (typeInfo.GenericTypeParameters.Any())
            {
                var genericTypeParameters = typeInfo.GenericTypeParameters
                    .Select(x => Get(x, collectionFormat).Name)
                    .Aggregate((x, y) => x + ", " + y);
                result += $"<{genericTypeParameters}>";
            }

            if (typeInfo.IsCollection)
            {
                isPrimitive = false;
                result = string.Format(collectionFormat ?? DefaultCollectionFormat, result);
            }

            return new ResolvedTypeInfo(result, isPrimitive, null);
        }
    }
}
