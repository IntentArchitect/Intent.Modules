#nullable enable
namespace Intent.Modules.Common.CSharp.CodeToModelSynchronization;

/// <summary>
/// Details of a current or generated node for <see cref="ICSharpSemanticComparisonNode"/>.
/// </summary>
public interface INodeDetails
{
    /// <summary>
    /// E.g. The name of the type declaration (class, interface, record), method, property, field, parameter, etc.
    /// </summary>
    string? Identifier { get; }

    /// <summary>
    /// The type e.g. int, string, etc. For fields, properties, parameters it's the type itself, for methods it's the return type, not applicable for type declarations such as classes, records, etc.
    /// </summary>
    string? TypeName { get; }

    /// <summary>
    /// The value of the node, e.g. for fields, properties, parameters it's the default value.
    /// </summary>
    string? Value { get; }
}