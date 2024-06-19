#nullable enable
using System;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

/// <summary>
/// Extensions methods for <see cref="IRazorFile"/> for when working with Blazor.
/// </summary>
public static class BlazorFileExtensions
{
    /// <summary>
    /// Adds an <c>@page</c> directive.
    /// </summary>
    public static IRazorFile AddPageDirective(this IRazorFile razorFile, string route)
    {
        razorFile.Directives.Insert(0, new RazorDirective("page", new CSharpStatement($"\"{route}\"")));
        return razorFile;
    }

    /// <summary>
    /// Adds an <c>@inherits</c> directive.
    /// </summary>
    public static IRazorFile AddInheritsDirective(this IRazorFile razorFile, string baseClassName)
    {
        razorFile.Directives.Add(new RazorDirective("inherits", new CSharpStatement(baseClassName)));
        return razorFile;
    }

    /// <summary>
    /// Adds an <c>@inject</c> directive.
    /// </summary>
    public static IRazorFile AddInjectDirective(this IRazorFile razorFile, string fullyQualifiedTypeName, string? propertyName = null)
    {
        var serviceDeclaration = $"{razorFile.Template.UseType(fullyQualifiedTypeName)} {propertyName ?? razorFile.Template.UseType(fullyQualifiedTypeName)}";
        if (!razorFile.Directives.Any(x => x.Keyword == "inject" && x.Expression?.ToString() == serviceDeclaration))
        {
            razorFile.Directives.Add(new RazorDirective("inject", new CSharpStatement(serviceDeclaration)));
        }
        return razorFile;
    }

    /// <summary>
    /// Adds an <c>@code</c> directive.
    /// </summary>
    public static IRazorFile AddCodeBlock(this IRazorFile razorFile, Action<IRazorCodeBlock>? configure = null)
    {
        var razorCodeBlock = new RazorCodeBlock(razorFile);
        razorFile.ChildNodes.Add(razorCodeBlock);
        configure?.Invoke(razorCodeBlock);
        return razorFile;
    }
}