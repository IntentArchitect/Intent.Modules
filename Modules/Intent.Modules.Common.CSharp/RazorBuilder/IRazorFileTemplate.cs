#nullable enable
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;

namespace Intent.Modules.Common.CSharp.RazorBuilder
{
    public interface IRazorFileTemplate : IFileBuilderTemplate, ICSharpTemplate
    {
        IRazorFile RazorFile { get; }
        IFileBuilderBase IFileBuilderTemplate.File => RazorFile;
    }
}