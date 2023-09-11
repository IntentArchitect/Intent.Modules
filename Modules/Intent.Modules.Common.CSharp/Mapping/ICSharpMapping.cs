using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Mapping;

public interface ICSharpMapping
{
    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappedEnd Mapping { get; set; }
    IEnumerable<CSharpStatement> GetMappingStatements();

    CSharpStatement GetSourceStatement();
    CSharpStatement GetTargetStatement();

    void SetSourceReplacement(IMetadataModel type, string replacement);
    void SetTargetReplacement(IMetadataModel type, string replacement);
}