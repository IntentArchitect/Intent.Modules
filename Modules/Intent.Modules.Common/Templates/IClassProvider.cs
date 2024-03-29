﻿using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    /// <summary>
    /// Template that provides a OOP class type - Class name and namespace (package).
    /// </summary>
    [FixFor_Version4("This should be ITypeProvider / ITypeProviderTemplate")]
    public interface IClassProvider : ITemplate
    {
        /// <summary>
        /// The namespace for the type defined by this class provider.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// The type name for the type defined by this class provider.
        /// </summary>
        string ClassName { get; }
    }

    /// <summary>
    /// Extension methods for <see cref="IClassProvider"/>.
    /// </summary>
    public static class IClassProviderExtensions
    {
        /// <summary>
        /// Returns the fully qualified class name.
        /// </summary>
        public static string FullTypeName(this IClassProvider item)
        {
            if (string.IsNullOrWhiteSpace(item.Namespace))
            {
                return item.ClassName;
            }
            return item.Namespace + "." + item.ClassName;
        }
    }
}
