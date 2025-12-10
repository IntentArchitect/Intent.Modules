#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.CodeToModelSynchronization;

public interface ICSharpSemanticComparisonNode
{
    IReadOnlyList<ICSharpSemanticComparisonNode> ChildNodes { get; }

    /// <summary>
    /// E.g. Added, Removed, Modified
    /// </summary>
    CSharpDifferenceType DifferenceType { get; }

    /// <summary>
    /// E.g. ClassDeclaration, MethodDeclaration, etc.
    /// </summary>
    CSharpSyntaxKind SyntaxKind { get; }

    /// <summary>
    /// E.g. the name of the method, class, property, etc.
    /// </summary>
    string? Identifier { get; }

    /// <summary>
    /// The old identifier before the change, if applicable.
    /// </summary>
    string? OldIdentifier { get; }

    /// <summary>
    /// The type name involved in the difference, if applicable. E.g. string, int, etc...
    /// </summary>
    string? TypeName { get; }

    /// <summary>
    /// The old type name before the change, if applicable.
    /// </summary>
    string? OldTypeName { get; }
}