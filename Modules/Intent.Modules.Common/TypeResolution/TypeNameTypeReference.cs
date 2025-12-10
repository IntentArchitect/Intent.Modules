using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.TypeResolution;

/// <inheritdoc/>
public class TypeNameTypeReference : ITypeNameTypeReference
{
    /// <inheritdoc cref="ITypeReference.IsNullable"/>
    public bool IsNullable { get; set; }

    /// <inheritdoc cref="ITypeReference.IsNullable"/>
    public bool IsCollection { get; set; }

    /// <inheritdoc cref="ITypeReference.GenericTypeParameters"/>
    public List<ITypeNameTypeReference> GenericTypeParameters { get; set; }

    /// <inheritdoc cref="ITypeReference.Element"/>
    public ICanBeReferencedType Element { get; set; }

    IEnumerable<ITypeReference> ITypeReference.GenericTypeParameters => GenericTypeParameters;

    IEnumerable<IStereotype> IHasStereotypes.Stereotypes { get; } = [];
}