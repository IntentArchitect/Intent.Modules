using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.AppStartup;

public interface IProgramTemplate : ICSharpFileBuilderTemplate
{
    IProgramFile ProgramFile { get; }
}