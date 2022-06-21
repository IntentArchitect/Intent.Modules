//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Intent.Metadata.Models;
//using Intent.Modules.Common.Templates;
//using Intent.Modules.Common.TypeResolution;

//namespace Intent.Modules.Common.Java.TypeResolvers
//{
//    /// <summary>
//    /// Java specialization of <see cref="TypeResolverBase"/>.
//    /// </summary>
//    public class JavaTypeResolverNew : TypeResolverBase, ITypeResolver
//    {
//        private static readonly Dictionary<string, (string PrimitiveTypeName, string WrapperTypeName)> PrimitivesTypeMap = new()
//        {
//            ["byte"] = ("byte", "Byte"),
//            ["short"] = ("short", "Short"),
//            ["int"] = ("int", "Integer"),
//            ["long"] = ("long", "Long"),
//            ["float"] = ("float", "Float"),
//            ["double"] = ("double", "Double"),
//            ["bool"] = ("boolean", "Boolean"),
//            ["boolean"] = ("boolean", "Boolean"),
//            ["char"] = ("char", "Character")
//        };

//        /// <summary>
//        /// Creates a new instance of <see cref="JavaTypeResolver"/>.
//        /// </summary>
//        public JavaTypeResolverNew() : base(defaultContext: new JavaTypeResolverContext())
//        {
//        }

//        /// <inheritdoc />
//        protected override ITypeResolverContext CreateContext()
//        {
//            return new JavaTypeResolverContext();
//        }

//        /// <summary>
//        /// Returns a non-primitive type version of the provided <paramref name="typeInfo"/>.
//        /// </summary>
//        /// <remarks>
//        /// Will return the same instance of <paramref name="typeInfo"/> if its
//        /// <see cref="ResolvedTypeInfo.IsPrimitive"/> value is <see langword="false"/>.
//        /// </remarks>
//        public static JavaResolvedTypeInfo ToNonPrimitive(JavaResolvedTypeInfo typeInfo)
//        {
//            if (!typeInfo.IsPrimitive || typeInfo.ArrayDimensionCount > 0)
//            {
//                return typeInfo;
//            }

//            return JavaResolvedTypeInfo.Create(
//                name: PrimitivesTypeMap[typeInfo.Name].WrapperTypeName,
//                package: typeInfo.Package,
//                isPrimitive: false,
//                isNullable: typeInfo.IsNullable,
//                isCollection: typeInfo.IsCollection,
//                typeReference: typeInfo.TypeReference,
//                template: typeInfo.Template,
//                genericTypeParameters: typeInfo.GenericTypeParameters);
//        }

//        /// <summary>
//        /// Retrieves the wrapped type name for the provided <paramref name="typeName"/> if it is a
//        /// primitive.
//        /// </summary>
//        /// <returns>
//        /// <see langword="true"/> if the provided <paramref name="typeName"/> is a primitive or
//        /// otherwise <see langword="false"/>.
//        /// </returns>
//        public static bool TryGetWrapperTypeName(string typeName, out string wrappedTypeName)
//        {
//            if (!PrimitivesTypeMap.TryGetValue(typeName, out var primitiveType))
//            {
//                wrappedTypeName = null;
//                return false;
//            }

//            wrappedTypeName = primitiveType.WrapperTypeName;
//            return true;
//        }

//        private class JavaTypeResolverContext : TypeResolverContextBase
//        {
//            private static readonly Dictionary<string, (string Package, string TypeName)> ObjectsTypeMap = new()
//            {
//                ["string"] = (string.Empty, "String"),
//                ["object"] = (string.Empty, "Object"),
//                ["datetime"] = ("java.time", "LocalDateTime"),
//                ["date"] = ("java.time", "LocalDate"),
//                ["decimal"] = ("java.math", "BigDecimal"),
//                ["datetimeoffset"] = ("java.time", "OffsetDateTime"),
//                ["guid"] = ("java.util", "UUID")
//            };

//            public JavaTypeResolverContext()
//                : base(JavaCollectionFormatter.GetOrCreate("{0}[]"), TypeResolution.DefaultNullableFormatter.Instance)
//            {
//            }

//            public override IResolvedTypeInfo Get(IClassProvider classProvider)
//            {
//                return JavaResolvedTypeInfo.Create(
//                    name: classProvider.ClassName,
//                    package: classProvider.Namespace,
//                    isPrimitive: false,
//                    isNullable: false,
//                    isCollection: false,
//                    typeReference: null,
//                    template: classProvider,
//                    genericTypeParameters: null);
//            }

//            protected override IResolvedTypeInfo Get(IResolvedTypeInfo resolvedTypeInfo) => GetInternal(resolvedTypeInfo);

//            private static JavaResolvedTypeInfo GetInternal(IResolvedTypeInfo resolvedTypeInfo)
//            {
//                return JavaResolvedTypeInfo.Create(resolvedTypeInfo);
//            }

//            public override IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat)
//            {
//                var collectionFormatter = !string.IsNullOrWhiteSpace(collectionFormat)
//                    ? JavaCollectionFormatter.GetOrCreate(collectionFormat)
//                    : null;

//                return Get(typeInfo, collectionFormatter);
//            }

//            public override IResolvedTypeInfo Get(ITypeReference typeReference, ICollectionFormatter collectionFormatter)
//            {
//                return GetInternal(typeReference, (JavaCollectionFormatter) collectionFormatter);
//            }

//            private JavaResolvedTypeInfo GetInternal(ITypeReference typeReference, JavaCollectionFormatter collectionFormatter)
//            {
//                if (typeReference == null)
//                {
//                    return null;
//                }

//                JavaResolvedTypeInfo resolvedTypeInfo = null;
//                foreach (var classLookup in TypeSources)
//                {
//                    var foundClass = classLookup.GetType(typeReference);
//                    if (foundClass == null)
//                    {
//                        continue;
//                    }

//                    collectionFormatter ??= (JavaCollectionFormatter)classLookup.CollectionFormatter;
//                    resolvedTypeInfo = GetInternal(foundClass);
//                    break;
//                }

//                collectionFormatter ??= (JavaCollectionFormatter)DefaultCollectionFormatter;

//                resolvedTypeInfo ??= ResolveTypeInternal(
//                    typeReference: typeReference,
//                    collectionFormatter: collectionFormatter);

//                if (typeReference.IsCollection)
//                {
//                    resolvedTypeInfo = collectionFormatter.ApplyTo(resolvedTypeInfo);
//                }

//                return resolvedTypeInfo;
//            }

//            protected override IResolvedTypeInfo ResolveType(
//                ITypeReference typeReference,
//                INullableFormatter nullableFormatter)
//            {
//                // Base methods calling this should have been overridden to use the overload which takes in an ICollectionFormatter
//                throw new InvalidOperationException("");
//            }

//            public override IResolvedTypeInfo Get(ITypeReference typeReference, ITypeSource typeSource)
//            {
//                return GetInternal(typeReference, typeSource);
//            }

//            private JavaResolvedTypeInfo GetInternal(ITypeReference typeReference, ITypeSource typeSource)
//            {
//                if (typeReference == null)
//                {
//                    return null;
//                }

//                JavaCollectionFormatter collectionFormatter = null;
//                JavaResolvedTypeInfo resolvedTypeInfo = null;
//                var foundClass = typeSource.GetType(typeReference);
//                if (foundClass != null)
//                {
//                    collectionFormatter = (JavaCollectionFormatter)typeSource.CollectionFormatter;
//                    resolvedTypeInfo = GetInternal(foundClass);
//                }
//                else
//                {
//                    foreach (var classLookup in TypeSources)
//                    {
//                        foundClass = classLookup.GetType(typeReference);
//                        if (foundClass == null)
//                        {
//                            continue;
//                        }

//                        collectionFormatter = (JavaCollectionFormatter)classLookup.CollectionFormatter;
//                        resolvedTypeInfo = GetInternal(foundClass);
//                        break;
//                    }
//                }

//                collectionFormatter ??= (JavaCollectionFormatter)DefaultCollectionFormatter;

//                resolvedTypeInfo ??= ResolveTypeInternal(
//                    typeReference: typeReference,
//                    collectionFormatter: collectionFormatter);

//                if (typeReference.IsCollection)
//                {
//                    resolvedTypeInfo = collectionFormatter.ApplyTo(resolvedTypeInfo);
//                }

//                return resolvedTypeInfo;
//            }

//            private JavaResolvedTypeInfo ResolveTypeInternal(
//                ITypeReference typeReference,
//                JavaCollectionFormatter collectionFormatter)
//            {
//                IReadOnlyList<JavaResolvedTypeInfo> ResolveGenericTypeParameters(IEnumerable<ITypeReference> genericTypeParameters)
//                {
//                    return genericTypeParameters
//                        .Select(reference => ToNonPrimitive(GetInternal(reference, collectionFormatter)))
//                        .ToArray();
//                }

//                if (typeReference.Element == null)
//                {
//                    return JavaResolvedTypeInfo.Create(
//                        name: "void",
//                        package: string.Empty,
//                        isPrimitive: true,
//                        isNullable: false,
//                        isCollection: false,
//                        typeReference: typeReference,
//                        template: null);
//                }

//                if (typeReference.Element.HasStereotype("Java"))
//                {
//                    var name = typeReference.Element.GetStereotypeProperty("Java", "Type", typeReference.Element.Name);
//                    var package = typeReference.Element.GetStereotypeProperty("Java", "Package", string.Empty);

//                    var lastIndexOfPeriod = name.LastIndexOf('.');
//                    if (lastIndexOfPeriod >= 0)
//                    {
//                        package = string.IsNullOrWhiteSpace(package)
//                            ? name[..lastIndexOfPeriod]
//                            : $"{package}.{name[..lastIndexOfPeriod]}";
//                        name = name[(lastIndexOfPeriod + 1)..];
//                    }

//                    return JavaResolvedTypeInfo.Create(
//                        name: name,
//                        package: package,
//                        isPrimitive: typeReference.Element.GetStereotypeProperty("Java", "Is Primitive", false),
//                        isNullable: typeReference.IsNullable,
//                        isCollection: typeReference.IsCollection,
//                        typeReference: typeReference,
//                        genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
//                }

//                if (PrimitivesTypeMap.TryGetValue(typeReference.Element.Name, out var primitiveType))
//                {
//                    return JavaResolvedTypeInfo.Create(
//                        name: !typeReference.IsNullable
//                            ? primitiveType.PrimitiveTypeName
//                            : primitiveType.WrapperTypeName,
//                        package: string.Empty,
//                        isPrimitive: true,
//                        isNullable: typeReference.IsNullable,
//                        isCollection: false,
//                        typeReference: typeReference);
//                }

//                if (ObjectsTypeMap.TryGetValue(typeReference.Element.Name, out var objectType))
//                {
//                    return JavaResolvedTypeInfo.Create(
//                        name: objectType.TypeName,
//                        package: objectType.Package,
//                        isPrimitive: false,
//                        isNullable: typeReference.IsNullable,
//                        isCollection: false,
//                        typeReference: typeReference);
//                }

//                if (typeReference.Element.Name == "binary")
//                {
//                    return JavaResolvedTypeInfo.CreateForArray(
//                        forResolvedType: JavaResolvedTypeInfo.Create(
//                            name: "byte",
//                            package: string.Empty,
//                            isPrimitive: true,
//                            isNullable: false,
//                            isCollection: false,
//                            typeReference: typeReference),
//                        isNullable: typeReference.IsNullable,
//                        arrayDimensionCount: 1);
//                }

//                return JavaResolvedTypeInfo.Create(
//                    name: typeReference.Element.Name,
//                    package: string.Empty,
//                    isPrimitive: false,
//                    isNullable: typeReference.IsNullable,
//                    isCollection: false,
//                    typeReference: typeReference,
//                    genericTypeParameters: ResolveGenericTypeParameters(typeReference.GenericTypeParameters));
//            }
//        }
//    }
//}
