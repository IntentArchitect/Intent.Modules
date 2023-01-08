using Intent.Modules.Common.TypeScript.Builder;

namespace Intent.Modules.Common.TypeScript.Templates;

public interface ITypescriptFileBuilderTemplate : ITypeScriptMerged
{
    TypescriptFile TypescriptFile { get; }
}