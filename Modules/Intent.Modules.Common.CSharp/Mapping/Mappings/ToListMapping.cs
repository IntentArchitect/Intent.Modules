using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.CSharp.Mapping;

public class ToListMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ToListMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
        _template = template;
    }

    public ToListMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
        _template = template;
    }

    public override CSharpStatement GetSourceStatement(bool? withNullConditionalOperators = null)
    {
        if((Mapping.SourceElement?.TypeReference?.IsCollection ?? false) != true && (Mapping.TargetElement?.TypeReference?.IsCollection ?? false) != true)
        {
            return base.GetSourceStatement(withNullConditionalOperators);
        }

        Template.AddUsing("System.Linq");
        return new CSharpAccessMemberStatement(base.GetSourceStatement(), new CSharpInvocationStatement("ToList").WithoutSemicolon());
    }
}
