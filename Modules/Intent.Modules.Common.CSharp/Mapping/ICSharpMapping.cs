using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public interface ICSharpMapping
{
    public ICanBeReferencedType Model { get; }
    public ICSharpMapping Parent { get; internal set; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappedEnd Mapping { get; set; }
    IEnumerable<CSharpStatement> GetMappingStatements();

    CSharpStatement GetSourceStatement();
    CSharpStatement GetTargetStatement();

    IDictionary<string, CSharpStatement> GetExpressionMap();

    bool TryGetSourceReplacement(IMetadataModel type, out string replacement);
    bool TryGetTargetReplacement(IMetadataModel type, out string replacement);

    internal void SetSourceReplacement(IMetadataModel type, string replacement);
    internal void SetTargetReplacement(IMetadataModel type, string replacement);
}

public interface ICSharpModelReplacement
{

}