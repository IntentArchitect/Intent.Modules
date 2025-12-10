#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.FactoryExtensions;
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
        private readonly ISoftwareFactoryExecutionContext? _executionContext;

        /// <summary>
        /// Creates a new instance of <see cref="CSharpTypeResolver"/>.
        /// </summary>
        public CSharpTypeResolver(
            CSharpCollectionFormatter defaultCollectionFormatter,
            INullableFormatter defaultNullableFormatter,
            ISoftwareFactoryExecutionContext? executionContext)
            : base(
                new CSharpTypeResolverContext(defaultCollectionFormatter, defaultNullableFormatter, executionContext))
        {
            _defaultCollectionFormatter = defaultCollectionFormatter;
            _defaultNullableFormatter = defaultNullableFormatter;
            _executionContext = executionContext;
        }

        /// <summary>
        /// Obsolete. Use <see cref="CSharpTypeResolver(CSharpCollectionFormatter,INullableFormatter,ISoftwareFactoryExecutionContext)"/> instead.
        /// </summary>
        public CSharpTypeResolver(
            CSharpCollectionFormatter defaultCollectionFormatter,
            INullableFormatter defaultNullableFormatter)
            : this(defaultCollectionFormatter, defaultNullableFormatter, null)
        {
            // Kept for binary compatibility
        }

        /// <inheritdoc />
        protected override ITypeResolverContext CreateContext()
        {
            return new CSharpTypeResolverContext(_defaultCollectionFormatter, _defaultNullableFormatter, _executionContext);
        }

        private class CSharpTypeResolverContext : TypeResolverContextBase<CSharpCollectionFormatter, CSharpResolvedTypeInfo>
        {
            private readonly ISoftwareFactoryExecutionContext? _executionContext;

            public CSharpTypeResolverContext(
                CSharpCollectionFormatter collectionFormatter,
                INullableFormatter nullableFormatter,
                ISoftwareFactoryExecutionContext? executionContext)
                : base(
                    collectionFormatter,
                    nullableFormatter)
            {
                _executionContext = executionContext;
            }

            private class TypeDefinition(string id, string name) : ICanBeReferencedType
            {
                public string Id { get; } = id;
                public IEnumerable<IStereotype> Stereotypes { get; } = [];
                public string SpecializationType => "Type-Definition";
                public string SpecializationTypeId => "d4e577cd-ad05-4180-9a2e-fff4ddea0e1e";
                public string Name { get; } = name;
                public string? Comment => null;
                public ITypeReference? TypeReference => null;
                public IPackage? Package => null;
            }

            private static readonly Dictionary<string, ICanBeReferencedType> TypeDefinitionsByCSharpName = new(StringComparer.OrdinalIgnoreCase)
            {
                ["byte[]"] = new TypeDefinition(ElementId.Binary, "binary"),
                ["Byte[]"] = new TypeDefinition(ElementId.Binary, "binary"),
                ["bool"] = new TypeDefinition(ElementId.Bool, "bool"),
                ["Bool"] = new TypeDefinition(ElementId.Bool, "bool"),
                ["byte"] = new TypeDefinition(ElementId.Byte, "byte"),
                ["Byte"] = new TypeDefinition(ElementId.Byte, "byte"),
                ["char"] = new TypeDefinition(ElementId.Char, "char"),
                ["Char"] = new TypeDefinition(ElementId.Char, "char"),
                ["DateOnly"] = new TypeDefinition(ElementId.Date, "date"),
                ["DateTime"] = new TypeDefinition(ElementId.DateTime, "datetime"),
                ["DateTimeOffset"] = new TypeDefinition(ElementId.DateTimeOffset, "datetimeoffset"),
                ["decimal"] = new TypeDefinition(ElementId.Decimal, "decimal"),
                ["Decimal"] = new TypeDefinition(ElementId.Decimal, "decimal"),
                ["double"] = new TypeDefinition(ElementId.Double, "double"),
                ["Double"] = new TypeDefinition(ElementId.Double, "double"),
                ["float"] = new TypeDefinition(ElementId.Float, "float"),
                ["Single"] = new TypeDefinition(ElementId.Float, "float"),
                ["Guid"] = new TypeDefinition(ElementId.Guid, "guid"),
                ["int"] = new TypeDefinition(ElementId.Int, "int"),
                ["Int32"] = new TypeDefinition(ElementId.Int, "int"),
                ["long"] = new TypeDefinition(ElementId.Long, "long"),
                ["Int64"] = new TypeDefinition(ElementId.Long, "long"),
                ["object"] = new TypeDefinition(ElementId.Object, "object"),
                ["short"] = new TypeDefinition(ElementId.Short, "short"),
                ["Int16"] = new TypeDefinition(ElementId.Short, "short"),
                ["string"] = new TypeDefinition(ElementId.String, "string"),
                ["String"] = new TypeDefinition(ElementId.String, "string"),
                ["Dictionary<,>"] = new TypeDefinition("ac2b65c3-6a8f-454a-b520-b583350c43ef", "Dictionary"),
                ["Task"] = new TypeDefinition("6cdaffae-7609-47ab-b8db-9d7282d2df6f", "Task"),
                ["Task<>"] = new TypeDefinition(ElementId.String, "Task<T>"),
                ["TimeSpan"] = new TypeDefinition(ElementId.String, "TimeSpan"),
            };

            private ITypeNameTypeReference? GetTypeReference(CSharpType cSharpType, IPackage package, out bool hadError)
            {
                switch (cSharpType)
                {
                    case CSharpTypeArray array:
                        {
                            var typeReference = GetTypeReference(array.ElementType, package, out hadError);
                            if (typeReference != null)
                            {
                                typeReference.IsCollection = true;
                            }

                            return typeReference;
                        }
                    case CSharpTypeGeneric generic:
                        {
                            if (generic.IsCollectionType() && generic.TypeArgumentList.Count == 1)
                            {
                                var typeReference = GetTypeReference(generic.TypeArgumentList[0], package, out hadError);
                                if (typeReference != null)
                                {
                                    typeReference.IsCollection = true;
                                }

                                return typeReference;
                            }
                            else
                            {
                                var genericTypeName = $"{generic.TypeName}<{string.Join(',', generic.TypeArgumentList.Select(_ => string.Empty))}>";
                                if (TryGetTypeReferenceCore(genericTypeName, package, out var typeReference) &&
                                    typeReference != null)
                                {
                                    var args = new List<ITypeNameTypeReference?>();

                                    hadError = false;
                                    foreach (var typeArg in generic.TypeArgumentList)
                                    {
                                        args.Add(GetTypeReference(typeArg, package, out var internalHadError));
                                        hadError |= internalHadError;
                                    }

                                    typeReference.GenericTypeParameters = args;
                                    return typeReference;
                                }

                                hadError = true;
                                return null;
                            }

                        }
                    case CSharpTypeName name:
                        {
                            if (TryGetTypeReferenceCore(name.TypeName, package, out var tr))
                            {
                                hadError = false;
                                return tr;
                            }

                            hadError = true;
                            return null;
                        }
                    case CSharpTypeNullable nullable:
                        {
                            var typeReference = GetTypeReference(nullable.ElementType, package, out hadError);
                            if (typeReference != null)
                            {
                                typeReference.IsNullable = true;
                            }

                            return typeReference;
                        }
                    case CSharpTypeTuple:
                        {
                            // No way in Intent to represent tuples at present
                            hadError = true;
                            return null;
                        }
                    case CSharpTypeVoid:
                        {
                            hadError = false;
                            return null;
                        }
                    default:
                        {
                            hadError = false;
                            return null;
                        }
                }
            }

            private bool TryGetTypeReferenceCore(string typeName, IPackage package, out ITypeNameTypeReference? typeReference)
            {
                typeName = typeName.Split('.')[^1];

                if (TypeDefinitionsByCSharpName.TryGetValue(typeName, out var typeDefinition))
                {
                    typeReference = new TypeNameTypeReference
                    {
                        IsNullable = false,
                        IsCollection = false,
                        GenericTypeParameters = [],
                        Element = typeDefinition
                    };
                    return true;
                }

                if (_executionContext != null &&
                    CSharpTypesCache.GetTypeDefinitionsByName(package, _executionContext.MetadataManager).TryGetValue(typeName, out var element))
                {
                    typeReference = new TypeNameTypeReference
                    {
                        IsNullable = false,
                        IsCollection = false,
                        GenericTypeParameters = [],
                        Element = element
                    };
                    return true;
                }

                if (base.TryGetTypeReference(typeName, package, out typeReference))
                {
                    return true;
                }

                typeReference = null;
                return false;
            }

            public override bool TryGetTypeReference(string typeName, IPackage package, out ITypeNameTypeReference? typeReference)
            {
                if (!CSharpTypeParser.TryParse(typeName, out var parsedType))
                {
                    typeReference = null;
                    return false;
                }

                // TODO JL: What can we do with hadError?
                typeReference = GetTypeReference(parsedType, package, hadError: out _);
                return true;
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

            protected override CSharpResolvedTypeInfo Get(
                IResolvedTypeInfo resolvedTypeInfo,
                IEnumerable<ITypeReference> genericTypeParameters,
                CSharpCollectionFormatter collectionFormatter)
            {
                return CSharpResolvedTypeInfo.Create(
                    resolvedTypeInfo: resolvedTypeInfo,
                    genericTypeParameters: genericTypeParameters
                        .Select(type => Get(type, collectionFormatter))
                        .ToArray());
            }

            protected override CSharpResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
            {
                var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
                    ? CSharpCollectionFormatter.Create(collectionFormat)
                    : null;

                return Get(typeInfo, collectionFormatter);
            }

            protected override CSharpResolvedTypeInfo ResolveType(
                ITypeReference typeReference,
                INullableFormatter nullableFormatter,
                CSharpCollectionFormatter collectionFormatter)
            {
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
                            jaggedArrays:
                            [
                                new CSharpJaggedArray()
                            ],
                            nullableFormatter);
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
                    case "date":
                        return CSharpResolvedTypeInfo.Create(
                            name: "DateOnly",
                            @namespace: "System",
                            isPrimitive: true,
                            isNullable: typeReference.IsNullable,
                            isCollection: false,
                            typeReference: typeReference,
                            nullableFormatter: nullableFormatter);
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
