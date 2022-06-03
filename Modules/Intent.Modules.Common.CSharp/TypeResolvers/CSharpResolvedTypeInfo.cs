using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.TypeResolvers
{
    /// <summary>
    /// C# specialization of <see cref="ResolvedTypeInfo"/>.
    /// </summary>
    public class CSharpResolvedTypeInfo : ResolvedTypeInfo
    {
        private CSharpResolvedTypeInfo(
            string name,
            string @namespace,
            bool isPrimitive,
            bool isNullable,
            bool isCollection,
            ITypeReference typeReference,
            ITemplate template,
            INullableFormatter nullableFormatter,
            IReadOnlyList<CSharpResolvedTypeInfo> genericTypeParameters,
            IReadOnlyList<CSharpJaggedArray> jaggedArrays)
            : base(
                name: name,
                isPrimitive: isPrimitive,
                isNullable: isNullable,
                isCollection: isCollection,
                typeReference: typeReference,
                template: template,
                nullableFormatter: nullableFormatter,
                collectionFormatter: null,
                genericTypeParameters: genericTypeParameters)
        {
            Namespace = @namespace;
            JaggedArrays = jaggedArrays;
        }

        /// <summary>
        /// Creates a new instance of <see cref="CSharpResolvedTypeInfo"/> for a non-array resolved type.
        /// </summary>
        public static CSharpResolvedTypeInfo Create(
            string name,
            string @namespace,
            bool isPrimitive,
            bool isNullable,
            bool isCollection,
            ITypeReference typeReference,
            INullableFormatter nullableFormatter,
            ITemplate template = null,
            IReadOnlyList<CSharpResolvedTypeInfo> genericTypeParameters = null)
        {
            return new CSharpResolvedTypeInfo(
                name: name,
                @namespace: @namespace,
                isPrimitive: isPrimitive,
                isNullable: isNullable,
                isCollection: isCollection,
                typeReference: typeReference,
                template: template,
                nullableFormatter: nullableFormatter,
                genericTypeParameters: genericTypeParameters ?? Array.Empty<CSharpResolvedTypeInfo>(),
                jaggedArrays: null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CSharpResolvedTypeInfo"/> from the provided <paramref name="resolvedTypeInfo"/>.
        /// </summary>
        public static CSharpResolvedTypeInfo Create(IResolvedTypeInfo resolvedTypeInfo)
        {
            var (name, @namespace) = resolvedTypeInfo.Template is IClassProvider classProvider
                ? (classProvider.ClassName, classProvider.Namespace)
                : (resolvedTypeInfo.Name, string.Empty);

            return new CSharpResolvedTypeInfo(
                name: name,
                @namespace: @namespace,
                isPrimitive: resolvedTypeInfo.IsPrimitive,
                isNullable: resolvedTypeInfo.IsNullable,
                isCollection: resolvedTypeInfo.IsCollection,
                typeReference: resolvedTypeInfo.TypeReference,
                template: resolvedTypeInfo.Template,
                nullableFormatter: resolvedTypeInfo.NullableFormatter,
                genericTypeParameters: resolvedTypeInfo.GenericTypeParameters.Select(Create).ToArray(),
                jaggedArrays: null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CSharpResolvedTypeInfo"/> for an array resolved type.
        /// </summary>
        public static CSharpResolvedTypeInfo CreateForArray(
            CSharpResolvedTypeInfo forResolvedType,
            bool isNullable,
            INullableFormatter nullableFormatter,
            IReadOnlyList<CSharpJaggedArray> jaggedArrays)
        {
            if ((jaggedArrays?.Count ?? 0) == 0)
            {
                throw new ArgumentException("Must be non-null and have at least one item", nameof(jaggedArrays));
            }

            return new CSharpResolvedTypeInfo(
                name: string.Empty,
                @namespace: string.Empty,
                isPrimitive: false,
                isNullable: isNullable,
                isCollection: true,
                typeReference: null,
                template: null,
                nullableFormatter: nullableFormatter,
                genericTypeParameters: new[] { forResolvedType },
                jaggedArrays: jaggedArrays);
        }

        /// <inheritdoc cref="IResolvedTypeInfo.WithIsNullable" />
        public new CSharpResolvedTypeInfo WithIsNullable(bool isNullable)
        {
            return (CSharpResolvedTypeInfo)WithIsNullableProtected(isNullable);
        }

        /// <inheritdoc />
        protected override IResolvedTypeInfo WithIsNullableProtected(bool isNullable)
        {
            return new CSharpResolvedTypeInfo(
                name: Name,
                @namespace: Namespace,
                isPrimitive: IsPrimitive,
                isNullable: isNullable,
                isCollection: IsCollection,
                typeReference: TypeReference,
                template: Template,
                nullableFormatter: NullableFormatter,
                genericTypeParameters: GenericTypeParameters,
                jaggedArrays: JaggedArrays);
        }

        /// <summary>
        /// The namespace for the type.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// The <see cref="CSharpJaggedArray"/> of the type, could be empty.
        /// </summary>
        public IReadOnlyList<CSharpJaggedArray> JaggedArrays { get; }

        /// <inheritdoc cref="IResolvedTypeInfo.GenericTypeParameters"/>
        public new IReadOnlyList<CSharpResolvedTypeInfo> GenericTypeParameters => (IReadOnlyList<CSharpResolvedTypeInfo>)base.GenericTypeParameters;

        /// <summary>
        /// Returns the namespace for this this type as well as the recursively acquired namespaces
        /// of this type's <see cref="GenericTypeParameters"/>.
        /// </summary>
        public IEnumerable<string> GetNamespaces()
        {
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                yield return Namespace;
            }

            foreach (var @namespace in GenericTypeParameters.SelectMany(x => x.GetNamespaces()))
            {
                yield return @namespace;
            }
        }

        /// <summary>
        /// Gets the fully qualified type name.
        /// </summary>
        public string GetFullyQualifiedTypeName()
        {
            var fullyQualifiedTypeName = string.IsNullOrWhiteSpace(Namespace)
                ? Name
                : $"{Namespace}.{Name}";

            fullyQualifiedTypeName = JaggedArrays != null
                ? $"{GenericTypeParameters[0].GetFullyQualifiedTypeName()}{string.Join(null, JaggedArrays.Select(x => x.ToString()))}"
                : GenericTypeParameters.Count > 0
                    ? $"{fullyQualifiedTypeName}<{string.Join(", ", GenericTypeParameters.Select(x => x.GetFullyQualifiedTypeName()))}>"
                    : fullyQualifiedTypeName;

            if (NullableFormatter != null)
            {
                fullyQualifiedTypeName = NullableFormatter.AsNullable(this, fullyQualifiedTypeName);
            }
             
            return fullyQualifiedTypeName;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var typeName = Name;

            typeName = JaggedArrays != null
                ? $"{GenericTypeParameters[0]}{string.Join(null, JaggedArrays.Select(x => x.ToString()))}"
                : GenericTypeParameters.Count > 0
                    ? $"{typeName}<{string.Join(", ", GenericTypeParameters.Select(x => x.ToString()))}>"
                    : typeName;

            if (NullableFormatter != null)
            {
                typeName = NullableFormatter.AsNullable(this, typeName);
            }

            return typeName;
        }
    }
}
