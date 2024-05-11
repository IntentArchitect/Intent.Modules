using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

/// <summary>
/// Well known C# code generation type values for <see cref="ITemplateFileConfig.CodeGenType"/>.
/// </summary>
public static class CodeGenType
{
    /// <summary>
    /// Roslyn Weaver.
    /// </summary>
    public static readonly string RoslynWeaver = nameof(RoslynWeaver);

    /// <summary>
    /// Razor Merger.
    /// </summary>
    public static readonly string RazorMerger = nameof(RazorMerger);
}