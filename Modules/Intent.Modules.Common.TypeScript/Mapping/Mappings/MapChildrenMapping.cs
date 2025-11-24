using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Typescript.Mapping;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;

namespace Intent.Modules.Common.Typescript.Mapping;

public class MapChildrenMapping : TypescriptMappingBase
{
    public MapChildrenMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ITypescriptFileBuilderTemplate template) : base(model, mapping, children, template)
    {
    }

    public MapChildrenMapping(MappingModel model, ITypescriptFileBuilderTemplate template) : base(model, template)
    {
    }

    public MapChildrenMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
    {
    }


    public override IEnumerable<TypescriptStatement> GetMappingStatements()
    {
        return Children.SelectMany(x => x.GetMappingStatements()).ToList();
    }

}