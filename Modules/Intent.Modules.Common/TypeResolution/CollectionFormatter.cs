using System;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Generic collection formatter implementation which accepts the collection formatter as a string or function.
    /// </summary>
    public class CollectionFormatter : ICollectionFormatter
    {
        private readonly Func<IResolvedTypeInfo, string> _formatCollection;

        public CollectionFormatter(string collectionFormat)
        {
            _formatCollection = (type) => string.Format(collectionFormat, type.Name);
        }

        public CollectionFormatter(Func<string, string> formatCollection)
        {
            _formatCollection = (type) => formatCollection(type.Name);
        }

        public CollectionFormatter(Func<IResolvedTypeInfo, string> formatCollection)
        {
            _formatCollection = formatCollection;
        }

        public string AsCollection(IResolvedTypeInfo typeInfo)
        {
            return _formatCollection(typeInfo);
        }
    }
}