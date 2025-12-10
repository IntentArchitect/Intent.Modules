using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// A mutable <see cref="ITypeReference"/> reverse engineered from
    /// a type name.
    /// </summary>
    public interface ITypeNameTypeReference : ITypeReference
    {
        /// <inheritdoc cref="ITypeReference.IsNullable"/>
        public new bool IsNullable { get; set; }

        /// <inheritdoc cref="ITypeReference.IsCollection"/>
        public new bool IsCollection { get; set; }

        /// <inheritdoc cref="ITypeReference.GenericTypeParameters"/>
        public new List<ITypeNameTypeReference> GenericTypeParameters { get; set; }

        /// <inheritdoc cref="ITypeReference.Element"/>
        public new ICanBeReferencedType Element { get; set; }
    }
}
