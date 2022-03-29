using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp
{
    /// <summary>
    /// A collection formatter that uses a specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CSharpCollectionFormatter<T> : CollectionFormatter
    {
        /// <summary>
        /// A collection formatter that uses the <paramref name="fullyQualifiedTypeName"/>.
        /// </summary>
        public CSharpCollectionFormatter(string fullyQualifiedTypeName, CSharpTemplateBase<T> template) : base((string type) => $"{template.UseType(fullyQualifiedTypeName)}<{type}>")
        {
        }
    }
}