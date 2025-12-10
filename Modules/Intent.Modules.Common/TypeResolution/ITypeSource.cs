#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Provides a source to resolve types.
    /// </summary>
    public interface ITypeSource
    {
        /// <summary>
        /// Returns a <see cref="IResolvedTypeInfo"/> type if this type source resolves a type. If not, this method should return null.
        /// </summary>
        /// <param name="typeReference"/>
        /// <returns></returns>
        IResolvedTypeInfo GetType(ITypeReference typeReference);

        /// <summary>
        /// Returns any template dependencies that were found when the <see cref="GetType"/> method was called.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITemplateDependency> GetTemplateDependencies();

        /// <summary>
        /// A formatter for if the <see cref="ITypeReference"/> provided to the <see cref="GetType"/> method is marked as a collection.
        /// </summary>
        ICollectionFormatter CollectionFormatter { get; }

        /// <summary>
        /// A formatter for if the <see cref="ITypeReference"/> provided to the <see cref="GetType"/> method is marked as nullable.
        /// </summary>
        INullableFormatter NullableFormatter { get; }

        /// <summary>
        /// The template ID that this type source is associated with (if any).
        /// </summary>
        string? TemplateId => null;

        /// <summary>
        /// Gets a type reference by name if it exists in this type source.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="typeReference"></param>
        /// <returns></returns>
        public bool TryGetTypeReference(string typeName, [NotNullWhen(true)] out ITypeNameTypeReference? typeReference)
        {
            throw new NotImplementedException($"{GetType().Name} does not have an implementation for {nameof(TryGetTypeReference)}");
        }
    }
}