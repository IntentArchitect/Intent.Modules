using System;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Razor;

public static class BlazorFileExtensions
{
    public static RazorFile AddPageDirective(this RazorFile razorFile, string route)
    {
        razorFile.Directives.Insert(0, new RazorDirective("page", new CSharpStatement($"\"{route}\"")));
        return razorFile;
    }

    public static RazorFile AddInheritsDirective(this RazorFile razorFile, string baseClassName)
    {
        razorFile.Directives.Add(new RazorDirective("inherits", new CSharpStatement(baseClassName)));
        return razorFile;
    }

    public static RazorFile AddInjectDirective(this RazorFile razorFile, string fullyQualifiedTypeName, string propertyName = null)
    {
        var serviceDeclaration = $"{razorFile.Template.UseType(fullyQualifiedTypeName)} {propertyName ?? razorFile.Template.UseType(fullyQualifiedTypeName)}";
        if (!razorFile.Directives.Any(x => x.Keyword == "inject" && x.Expression.ToString() == serviceDeclaration))
        {
            razorFile.Directives.Add(new RazorDirective("inject", new CSharpStatement(serviceDeclaration)));
        }
        return razorFile;
    }

    public static RazorFile AddCodeBlock(this RazorFile razorFile, Action<RazorCodeBlock> configure = null)
    {
        var razorCodeBlock = new RazorCodeBlock(razorFile);
        razorFile.ChildNodes.Add(razorCodeBlock);
        configure?.Invoke(razorCodeBlock);
        return razorFile;
    }
}