using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp
{
    /// <summary>
    /// Common CSharp collection formatters
    /// </summary>
    public static class CollectionFormatters
    {
        /// <summary>
        /// Formats collections using <see cref="System.Collections.Generic.List{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <returns></returns>
        public static ICollectionFormatter CreateListFormatter<T>(this CSharpTemplateBase<T> template)
        {
            return new CSharpCollectionFormatter<T>("System.Collections.Generic.List", template);
        }

        /// <summary>
        /// Formats collections using <see cref="System.Collections.Generic.IList{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <returns></returns>
        public static ICollectionFormatter CreateIListFormatter<T>(this CSharpTemplateBase<T> template)
        {
            return new CSharpCollectionFormatter<T>("System.Collections.Generic.IList", template);
        }

        /// <summary>
        /// Formats collections using <see cref="System.Collections.Generic.ICollection{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <returns></returns>
        public static ICollectionFormatter CreateICollectionFormatter<T>(this CSharpTemplateBase<T> template)
        {
            return new CSharpCollectionFormatter<T>("System.Collections.Generic.ICollection", template);
        }
    }
}