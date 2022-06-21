using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    /// <summary>
    /// C# specialization of <see cref="TypeResolverBase"/>.
    /// </summary>
    public class CSharpTypeResolver : TypeResolverBase
    {
        private readonly CSharpCollectionFormatter _defaultCollectionFormatter;
        private readonly INullableFormatter _defaultNullableFormatter;
        private readonly ICSharpProject _csharpProject;

        /// <summary>
        /// Creates a new instance of <see cref="CSharpTypeResolver"/>.
        /// </summary>
        public CSharpTypeResolver(
            CSharpCollectionFormatter defaultCollectionFormatter,
            INullableFormatter defaultNullableFormatter,
            ICSharpProject csharpProject)
            : base(
                new CSharpTypeResolverContext(defaultCollectionFormatter, defaultNullableFormatter, csharpProject))
        {
            _defaultCollectionFormatter = defaultCollectionFormatter;
            _defaultNullableFormatter = defaultNullableFormatter;
            _csharpProject = csharpProject;
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new CSharpTypeResolverContext(_defaultCollectionFormatter, _defaultNullableFormatter, _csharpProject);
        }

        private class CSharpTypeResolverContext : TypeResolverContextBase<CSharpCollectionFormatter, CSharpResolvedTypeInfo>
        {
            private readonly ICSharpProject _csharpProject;

            public CSharpTypeResolverContext(
                CSharpCollectionFormatter collectionFormatter,
                INullableFormatter nullableFormatter,
                ICSharpProject csharpProject)
                : base(
                    collectionFormatter,
                    nullableFormatter)
            {
                _csharpProject = csharpProject;
            }

            protected override CSharpResolvedTypeInfo Get(IClassProvider classProvider)
            {
                return CSharpResolvedTypeInfo.Create(
                    name: classProvider.ClassName,
                    @namespace: classProvider.Namespace,
                    isPrimitive: false,
                    isNullable: false,
                    isCollection: false,
                    typeReference: null,
                    nullableFormatter: null,
                    template: classProvider,
                    genericTypeParameters: null);
            }

            protected override CSharpResolvedTypeInfo Get(IResolvedTypeInfo resolvedTypeInfo)
            {
                return CSharpResolvedTypeInfo.Create(resolvedTypeInfo);
            }

            protected override CSharpResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
            {
                var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                    ? CSharpCollectionFormatter.GetOrCreate(collectionFormat)
                    : null;

                return Get(typeInfo, collectionFormatter);
            }

            protected override CSharpResolvedTypeInfo ResolveType(
                ITypeReference typeReference,
                INullableFormatter nullableFormatter,
                CSharpCollectionFormatter collectionFormatter)
            {
                static bool IsAtLeastDotNet6(ICSharpProject project)
                {
                    return project.TargetDotNetFrameworks
                        .DefaultIfEmpty()
                        .Max() >= Version.Parse("6.0");
                }

                IReadOnlyList<CSharpResolvedTypeInfo> ResolveGenericTypeParameters(IEnumerable<ITypeReference> genericTypeParameters)
                {
                    return genericTypeParameters
                        .Select(type => Get(type, collectionFormatter))
                        .ToArray();
                }

                if (typeReference.Element == null)
                {
                    return CSharpResolvedTypeInfo.Create(
                        name: "void",
                        @namespace: string.Empty,
                        isPrimitive: true,
                        isNullable: false,
                        isCollection: false,
                        typeReference: typeReference,
                        nullableFormatter: nullableFormatter,
                        template: null);
                }

                if (typeReference.Element.Stereotypes.Any(x => x.Name == "C#"))
                {
                    var name = typeReference.Element.GetStereotypeProperty("C#", "Type", typeReference.Element.Name);
                    var @namespace = typeReference.Element.GetStereotypeProperty("C#", "Namespace", string.Empty);

                    var lastIndexOfPeriod = name.LastIndexOf('.');
                    if (lastIndexOfPeriod >= 0)
                    {
                        @namespace = string.IsNullOrWhiteSpace(@namespace)
                            ? name[..lastIndexOfPeriod]
                            : $"{@namespace}.{name[..lastIndexOfPeriod]}";
                        name = name[(lastIndexOfPeriod + 1)..];
                    }

                    return CSharpResolvedTypeInfo.Create(
                        name: name,
                        @namespace: @namespace,
                        isPrimitive: typeReference.Element.GetStereotypeProperty("C#", "Is Primitive", true),
                        isNullable: typeReference.IsNullable,
                        isCollection: typeReference.Element.GetStereotypeProperty("C#", "Is Collection", typeReference.IsCollection),
                        typeReference: typeReference,
                        nullableFormatter: nullableFormatter,
                        genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
                }

                switch (typeReference.Element.Name)
                {
                    case "bool":
                    case "char":
                    case "byte":
                    case "decimal":
                    case "double":
                    case "float":
                    case "short":
                    case "int":
                    case "long":
                        return CSharpResolvedTypeInfo.Create(
                            name: typeReference.Element.Name,
                            @namespace: string.Empty,
                            isPrimitive: true,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            nullableFormatter: nullableFormatter,
                            typeReference: typeReference);
                    case "binary":
                        return CSharpResolvedTypeInfo.CreateForArray(
                            forResolvedType: CSharpResolvedTypeInfo.Create(
                                name: "byte",
                                @namespace: string.Empty,
                                isPrimitive: true,
                                isNullable: false,
                                isCollection: false,
                                nullableFormatter: nullableFormatter,
                                typeReference: typeReference),
                            isNullable: typeReference.IsNullable,
                            nullableFormatter: nullableFormatter,
                            jaggedArrays: new[]
                            {
                                new CSharpJaggedArray()
                            });
                    case "object":
                    case "string":
                        return CSharpResolvedTypeInfo.Create(
                            name: typeReference.Element.Name,
                            @namespace: string.Empty,
                            isPrimitive: false,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter);
                    case "guid":
                        return CSharpResolvedTypeInfo.Create(
                            name: "Guid",
                            @namespace: "System",
                            isPrimitive: true,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter);
                    case "datetimeoffset":
                        return CSharpResolvedTypeInfo.Create(
                            name: "DateTimeOffset",
                            @namespace: "System",
                            isPrimitive: true,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter);
                    case "date" when IsAtLeastDotNet6(_csharpProject):
                        return CSharpResolvedTypeInfo.Create(
                            name: "DateOnly",
                            @namespace: "System",
                            isPrimitive: true,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter);
                    case "date":
                    case "datetime":
                        return CSharpResolvedTypeInfo.Create(
                            name: "DateTime",
                            @namespace: "System",
                            isPrimitive: true,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter);
                    default:
                        return CSharpResolvedTypeInfo.Create(
                            name: typeReference.Element.Name,
                            @namespace: string.Empty,
                            isPrimitive: false,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter,
                            genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
                }
            }
        }
    }
}
