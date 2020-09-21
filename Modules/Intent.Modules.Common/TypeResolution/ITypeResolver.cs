using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public interface ITypeSource
    {
        string GetType(ITypeReference typeInfo);
        IEnumerable<ITemplateDependency> GetTemplateDependencies();
    }

    public interface ITypeResolver
    {
        /// <summary>
        /// Default format to use when the typeInfo.IsCollection is true;
        /// </summary>
        string DefaultCollectionFormat { get; set; }
        
        /// <summary>
        /// Adds a default <see cref="ITypeSource"/> that is used when resolving type names of classes.
        /// </summary>
        /// <param name="typeSource"></param>
        void AddClassTypeSource(ITypeSource typeSource);

        /// <summary>
        /// Adds an <see cref="ITypeSource"/> that is only used to resolve type names when <see cref="InContext(string)"/> is called for the specific <see cref="contextName"/>.
        /// </summary>
        /// <param name="typeSource"></param>
        /// <param name="contextName"></param>
        void AddClassTypeSource(ITypeSource typeSource, string contextName);

        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="collectionFormat">The collection format provided if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo, string collectionFormat);

        /// <summary>
        /// Resolves the type name for the specified <see cref="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        string Get(IElement element);

        /// <summary>
        /// Resolves the type name for the specified <see cref="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="collectionFormat">The collection format provided if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        string Get(IElement element, string collectionFormat);

        /// <summary>
        /// Returns a <see cref="ITypeResolverContext"/> that resolves the type using the <see cref="IClassTypeSource"/> added to the specified "<paramref name="contextName"/>"
        /// </summary>
        /// <param name="contextName"></param>
        /// <returns></returns>
        ITypeResolverContext InContext(string contextName);

        /// <summary>
        /// Returns a collection of template dependencies discovered while discovering type names from templates.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITemplateDependency> GetTemplateDependencies();
    }

    public interface ITypeResolverContext
    {
        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo);

        /// <summary>
        /// Resolves the type name for the specified <see cref="typeInfo"/>
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="collectionFormat">The collection type provided if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo, string collectionFormat);

    }
}
