using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Information about the resolved type.
    /// </summary>
    public interface IResolvedTypeInfo
    {
        /// <summary>
        /// The resolved name of the type.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Whether this type is a primitive type.
        /// </summary>
        bool IsPrimitive { get; }
        /// <summary>
        /// Whether this type is nullable
        /// </summary>
        bool IsNullable { get; }
        /// <summary>
        /// Whether this type is a collection
        /// </summary>
        bool IsCollection { get; }
        /// <summary>
        /// The template that was used to resolve this type, if any.
        /// <para>
        /// See <see cref="TypeResolverBase.AddTypeSource(Intent.Modules.Common.TypeResolution.ITypeSource)"/> for adding Type Sources for resolving these types.
        /// </para>
        /// </summary>
        ITemplate Template { get; }
        /// <summary>
        /// The original <see cref="ITypeReference"/> that was provided to resolve this type.
        /// </summary>
        ITypeReference TypeReference { get; }
        /// <summary>
        /// Resolved generic types for this <see cref="IResolvedTypeInfo"/>
        /// </summary>
        IList<IResolvedTypeInfo> GenericTypes { get; }
    }

    public class ResolvedTypeInfo : IResolvedTypeInfo
    {
        public ResolvedTypeInfo(string name, bool isPrimitive, ITypeReference typeReference, ITemplate template)
        {
            Name = name;
            IsPrimitive = isPrimitive && !typeReference.IsCollection && !typeReference.IsNullable;
            IsNullable = typeReference.IsNullable;
            IsCollection = typeReference.IsCollection;
            TypeReference = typeReference;
            Template = template;
        }

        public ResolvedTypeInfo(IResolvedTypeInfo typeInfo) : this(typeInfo.Name, typeInfo.IsPrimitive, typeInfo.TypeReference, typeInfo.Template)
        {

        }

        public string Name { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsNullable { get; set; }
        public bool IsCollection { get; set; }
        public ITemplate Template { get; set; }
        public ITypeReference TypeReference { get; set; }
        public IList<IResolvedTypeInfo> GenericTypes { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}