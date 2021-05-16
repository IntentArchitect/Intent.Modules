using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    public interface ITypeResolverContext
    {
        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        IResolvedTypeInfo Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="collectionFormat">The collection type provided if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat);

        /// <summary>
        /// Adds a <see cref="ITypeSource"/> that is used when resolving information about types provided by other templates.
        /// </summary>
        /// <param name="typeSource"></param>
        void AddTypeSource(ITypeSource typeSource);

        /// <summary>
        /// Default collection formatter to use when the typeInfo.IsCollection is true;
        /// </summary>
        void SetCollectionFormatter(ICollectionFormatter formatter);
        
        /// <summary>
        /// Returns the list of added <see cref="ITypeSource"/>s
        /// </summary>
        IEnumerable<ITypeSource> TypeSources { get; }
    }
}