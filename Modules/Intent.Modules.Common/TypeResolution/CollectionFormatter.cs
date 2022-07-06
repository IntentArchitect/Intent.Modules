using System;
using System.Collections.Concurrent;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Generic collection formatter implementation which accepts the collection formatter as a string or function.
    /// </summary>
    public class CollectionFormatter : ICollectionFormatter
    {
        private static readonly ConcurrentDictionary<string, CollectionFormatter> Cache = new();
        private readonly Func<string, string> _formatter;

        private CollectionFormatter(string collectionFormat)
        {
            _formatter = typeName => string.Format(collectionFormat, typeName);
        }

        /// <summary>
        /// Creates a new instance of <see cref="CollectionFormatter"/>.
        /// </summary>
        public CollectionFormatter(Func<string, string> formatCollection)
        {
            _formatter = formatCollection;
        }

        /// <inheritdoc />
        public virtual IResolvedTypeInfo ApplyTo(IResolvedTypeInfo typeInfo)
        {
            return ResolvedTypeInfo.CreateForCollection(
                forResolvedType: typeInfo.WithIsNullable(false),
                isNullable: typeInfo.IsNullable,
                nullableFormatter: typeInfo.NullableFormatter,
                collectionFormatter: this);
        }

        /// <summary>
        /// Returns an instance of <see cref="CollectionFormatter"/> with the specified
        /// <paramref name="collectionFormat"/>.
        /// </summary>
        /// <remarks>
        /// A cache of <see cref="CollectionFormatter"/> instances is first checked for an already
        /// existing instance, if an instance is found then that is returned, otherwise a new
        /// instance is created, placed in the cache and returned.
        /// </remarks>
        public static CollectionFormatter Create(string collectionFormat)
        {
            return Cache.GetOrAdd(collectionFormat, _ => new CollectionFormatter(collectionFormat));
        }

        /// <summary>
        /// Obsolete. Use <see cref="Create"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static CollectionFormatter GetOrCreate(string collectionFormat)
        {
            return Cache.GetOrAdd(collectionFormat, _ => new CollectionFormatter(collectionFormat));
        }

        /// <inheritdoc />
        public virtual string Format(string typeName)
        {
            return _formatter(typeName);
        }
    }
}