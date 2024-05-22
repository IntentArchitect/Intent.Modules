using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class MapChildrenMapping : CSharpMappingBase
{
    public MapChildrenMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
    }

    public MapChildrenMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
    }

    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        return Children.SelectMany(x => x.GetMappingStatements()).ToList();
    }

}