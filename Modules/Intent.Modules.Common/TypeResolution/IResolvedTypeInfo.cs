using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Information about the resolved type.
    /// </summary>
    public interface IResolvedTypeInfo
    {
        /// <summary>
        /// The resolved name of the type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether this type is a primitive type.
        /// </summary>
        bool IsPrimitive { get; }

        /// <summary>
        /// Whether this type is nullable
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Whether this type is a collection
        /// </summary>
        bool IsCollection { get; }

        /// <summary>
        /// The template that was used to resolve this type, if any.
        /// <para>
        /// See <see cref="TypeResolverBase.AddTypeSource(ITypeSource)"/> for adding Type Sources for resolving these types.
        /// </para>
        /// </summary>
        ITemplate Template { get; }

        /// <summary>
        /// The original <see cref="ITypeReference"/> that was provided to resolve this type.
        /// </summary>
        ITypeReference TypeReference { get; }

        /// <summary>
        /// Resolved generic types for this <see cref="IResolvedTypeInfo"/>.
        /// </summary>
        IReadOnlyList<IResolvedTypeInfo> GenericTypeParameters { get; }

        ///// <summary>
        ///// Collection formatter to apply to the type.
        ///// </summary>
        //ICollectionFormatter CollectionFormatter { get; }

        /// <summary>
        /// Nullable formatter to apply to the type.
        /// </summary>
        INullableFormatter NullableFormatter { get; }

        /// <summary>
        /// Finds all templates this resolved type is dependent on by checking itself as well as
        /// recursively checking generic type parameters as well.
        /// </summary>
        IEnumerable<ITemplate> GetTemplateDependencies();
    }
}