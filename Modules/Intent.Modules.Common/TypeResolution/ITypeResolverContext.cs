using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    public interface ITypeResolverContext
    {
        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        IResolvedTypeInfo Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="collectionFormat">The collection format to be applied if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        IResolvedTypeInfo Get(ITypeReference typeInfo, string collectionFormat);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="collectionFormatter">The collection formatter to be applied if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        IResolvedTypeInfo Get(ITypeReference typeInfo, ICollectionFormatter collectionFormatter);

        /// <summary>
        /// Resolves the type name for the specified <paramref name="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="typeSource">The <paramref name="typeSource"/> to search for the <paramref name="typeInfo"/>. If not found, will then search added <see cref="TypeSources"/>.</param>
        /// <returns></returns>
        IResolvedTypeInfo Get(ITypeReference typeInfo, ITypeSource typeSource);

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