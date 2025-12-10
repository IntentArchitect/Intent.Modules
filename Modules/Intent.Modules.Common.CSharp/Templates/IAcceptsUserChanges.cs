#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Templates;

public interface IAcceptsUserChanges
{
    void Apply(ICSharpDifferenceNode compilationUnitNode);
}

public interface ICSharpDifferenceNode
{
    IReadOnlyList<ICSharpDifferenceNode> Children { get; }

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

public enum CSharpDifferenceType
{
    Unchanged = 1,
    Added = 2,
    Removed = 3,
    Changed = 4
}

/// <summary>
/// The values of these needs to match the values as <see href="https://learn.microsoft.com/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind"/> as they are cast.
/// </summary>
public enum CSharpSyntaxKind
{
    CompilationUnit = 8840,
    NamespaceDeclaration = 8842,
    UsingDirective = 8843,

    ClassDeclaration = 8855,
    StructDeclaration = 8856,
    InterfaceDeclaration = 8857,
    EnumDeclaration = 8858,
    DelegateDeclaration = 8859,

    MethodDeclaration = 8875,
    ConstructorDeclaration = 8878,
    PropertyDeclaration = 8892,
    FieldDeclaration = 8873,
    EventDeclaration = 8893,

    Parameter = 8908
}