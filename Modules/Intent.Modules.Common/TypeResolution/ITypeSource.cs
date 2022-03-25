using System.Collections.Generic;
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
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        IResolvedTypeInfo GetType(ITypeReference typeInfo);

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
    }
}