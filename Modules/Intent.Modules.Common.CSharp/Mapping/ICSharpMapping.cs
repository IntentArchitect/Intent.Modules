using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Common.CSharp.Mapping;

public interface ICSharpMapping
{
    public ICanBeReferencedType Model { get; }
    public IList<ICSharpMapping> Children { get; }
    public IElementToElementMappingConnection Mapping { get; set; }
    IEnumerable<CSharpStatement> GetMappingStatements();

    CSharpStatement GetFromStatement();
    CSharpStatement GetToStatement();

    void SetFromReplacement(IMetadataModel type, string replacement);
    void SetToReplacement(IMetadataModel type, string replacement);
}