using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Common.CSharp.Mapping;

public class DefaultCSharpMapping : CSharpMappingBase
{
    public DefaultCSharpMapping(ICanBeReferencedType model, IElementToElementMappedEnd mapping, IList<MappingModel> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children, template)
    {
    }

    public DefaultCSharpMapping(MappingModel model, ICSharpFileBuilderTemplate template) : base(model, template)
    {
    }

    public override CSharpStatement GetSourceStatement()
    {
        if (!Mapping.IsOneToOne() || Mapping.TargetElement.TypeReference == null || Mapping.SourceElement.TypeReference == null)
        {
            return base.GetSourceStatement();
        }

        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.HasStringType() == true &&
            Mapping.SourceElement.TypeReference.HasStringType() == false)
        {
            return new CSharpInvocationStatement(base.GetSourceStatement(), "ToString").WithoutSemicolon();
        }
        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.Element?.SpecializationType == "Enum" && 
            Mapping.SourceElement.TypeReference.HasIntType())
        {
            return $"({Template.GetTypeName((IElement)Mapping.TargetElement.TypeReference.Element)}){base.GetSourceStatement()}";
        }

        if (Mapping.IsOneToOne() &&
            Mapping.SourceElement.TypeReference.IsNullable &&
            !Mapping.TargetElement.TypeReference.IsNullable &&
            Mapping.TargetElement.TypeReference.Element.IsTypeDefinitionModel())
        {
            return new CSharpAccessMemberStatement(base.GetSourceStatement(), "Value");
        }
        return base.GetSourceStatement();
    }
}

public static class ElementToElementMappedEndExtensions
{
    public static bool IsOneToOne(this IElementToElementMappedEnd model)
    {
        return model.Sources.Count() == 1 && model.MappingExpression.Trim() == $"{{{model.Sources.Single().ExpressionIdentifier}}}";
    }
}