using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.TypeScript.Builder;

namespace Intent.Modules.Common.Typescript.Mapping;

public interface ITypescriptMapping
{
    public ICanBeReferencedType Model { get; }
    public ITypescriptMapping Parent { get; internal set; }
    public IList<ITypescriptMapping> Children { get; }
    public IElementToElementMappedEnd Mapping { get; set; }
    IEnumerable<TypescriptStatement> GetMappingStatements();

    TypescriptStatement GetSourceStatement(bool? withNullConditionalOperators = default);
    //TypescriptStatement GetTargetStatement();
    //TypescriptStatement GetTargetStatement(bool withNullConditionalOperators);

    IDictionary<string, TypescriptStatement> GetExpressionMap();

    bool TryGetSourceReplacement(IMetadataModel type, out string replacement);
    bool TryGetTargetReplacement(IMetadataModel type, out string replacement);

    internal void SetSourceReplacement(IMetadataModel type, string replacement);
    internal void SetTargetReplacement(IMetadataModel type, string replacement);
}

public interface ITypescriptModelReplacement
{

}