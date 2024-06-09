using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Razor
{
    public interface IRazorFileTemplate : ICSharpTemplate
    {
        RazorFile RazorFile { get; }
    }
}