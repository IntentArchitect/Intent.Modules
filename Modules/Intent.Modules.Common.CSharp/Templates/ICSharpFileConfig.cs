using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates;

public interface ICSharpFileConfig : ITemplateFileConfig
{
    string ClassName { get; }
    string Namespace { get; }
}