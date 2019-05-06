using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    public interface IClassTypeSource
    {
        string GetClassType(ITypeReference typeInfo);
        IEnumerable<ITemplateDependency> GetTemplateDependencies();
    }

    public interface ITypeResolver
    {
        /// <summary>
        /// Adds a default <see cref="IClassTypeSource"/> that is used when resolving type names of classes.
        /// </summary>
        /// <param name="classTypeSource"></param>
        void AddClassTypeSource(IClassTypeSource classTypeSource);

        /// <summary>
        /// Adds an <see cref="IClassTypeSource"/> that is only used to resolve type names when <see cref="InContext(string)"/> is called for the specific <see cref="contextName"/>.
        /// </summary>
        /// <param name="classTypeSource"></param>
        /// <param name="contextName"></param>
        void AddClassTypeSource(IClassTypeSource classTypeSource, string contextName);

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
        /// <param name="collectionType">The collection type provided if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo, string collectionType);

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
        /// <param name="collectionType">The collection type provided if the typeInfo.IsCollection is true</param>
        /// <returns></returns>
        string Get(ITypeReference typeInfo, string collectionType);

    }
}
