using Intent.Modules.Common.TypeScript.Builder;

namespace Intent.Modules.Common.TypeScript.Templates;

public interface ITypescriptFileBuilderTemplate : ITypeScriptMerged, ITypescriptTemplate
{
    TypescriptFile TypescriptFile { get; }
}