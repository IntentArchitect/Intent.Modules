using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Mapping;

public class DefaultCSharpMapping : CSharpMappingBase
{
    public DefaultCSharpMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
    }

    public DefaultCSharpMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetFromStatement()
    {
        if (Mapping.ToPath.Last().Element.TypeReference?.HasStringType() == true && Mapping.FromPath.Last().Element.TypeReference.HasStringType() == false)
        {
            return new CSharpInvocationStatement(base.GetFromStatement(), "ToString").WithoutSemicolon();
        }
        if (Mapping.ToPath.Last().Element.TypeReference?.Element.SpecializationType == "Enum" && Mapping.FromPath.Last().Element.TypeReference.HasIntType())
        {
            return $"({Template.GetTypeName((IElement)Mapping.ToPath.Last().Element.TypeReference.Element)}){base.GetFromStatement()}";
        }
        return base.GetFromStatement();
    }
}