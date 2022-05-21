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
        /// If <see cref="Template"/> has <c>Namespace</c> in its <see cref="IFileMetadata.CustomMetadata"/>,
        /// then this returns the <see cref="Name"/> prefixed with the <c>Namespace</c> and <c>.</c>. Otherwise
        /// it returns just <see cref="Name"/>.
        /// </summary>
        string FullTypeName
        {
            get
            {
                if (Template?.GetMetadata().CustomMetadata.TryGetValue("Namespace", out var @namespace) != true ||
                    string.IsNullOrWhiteSpace(@namespace))
                {
                    return Name;
                }

                return $"{@namespace}.{Name}";
            }
        }

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
        /// See <see cref="TypeResolverBase.AddTypeSource(ITypeSource)"/> for adding Type Sources for resolving these types.
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

    /// <summary>
    /// Implementation of <see cref="IResolvedTypeInfo"/>.
    /// </summary>
    public class ResolvedTypeInfo : IResolvedTypeInfo
    {
        /// <summary>
        /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
        /// </summary>
        public ResolvedTypeInfo(string name, bool isPrimitive, bool isNullable, bool isCollection, ITypeReference typeReference, ITemplate template)
        {
            Name = name;
            IsPrimitive = isPrimitive && !typeReference.IsCollection;
            IsNullable = isNullable;
            IsCollection = isCollection;
            TypeReference = typeReference;
            Template = template;
        }

        /// <summary>
        /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
        /// </summary>
        public ResolvedTypeInfo(string name, bool isPrimitive, ITypeReference typeReference, ITemplate template) : this(name, isPrimitive, typeReference.IsNullable, typeReference.IsCollection, typeReference, template)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="ResolvedTypeInfo"/>.
        /// </summary>
        public ResolvedTypeInfo(IResolvedTypeInfo typeInfo) : this(typeInfo.Name, typeInfo.IsPrimitive, typeInfo.IsNullable, typeInfo.IsCollection, typeInfo.TypeReference, typeInfo.Template)
        {
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool IsPrimitive { get; set; }

        /// <inheritdoc />
        public bool IsNullable { get; set; }

        /// <inheritdoc />
        public bool IsCollection { get; set; }

        /// <inheritdoc />
        public ITemplate Template { get; set; }

        /// <inheritdoc />
        public ITypeReference TypeReference { get; set; }

        /// <inheritdoc />
        public IList<IResolvedTypeInfo> GenericTypes { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}