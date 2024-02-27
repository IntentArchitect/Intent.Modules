using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.TypeScript.Templates;

public static class TypescriptFileHelper
{
    public static string GetRelativePath<T>(this TypeScriptTemplateBase<T> template, ITemplate dependency)
    {
        return "./" + template.GetMetadata().GetFullLocationPath().GetRelativePath(dependency.GetMetadata().GetFilePathWithoutExtension());
    }
}