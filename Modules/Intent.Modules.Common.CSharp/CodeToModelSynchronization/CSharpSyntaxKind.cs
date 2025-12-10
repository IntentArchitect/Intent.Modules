#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.CSharp.CodeToModelSynchronization;

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