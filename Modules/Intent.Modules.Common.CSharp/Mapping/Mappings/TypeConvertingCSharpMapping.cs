using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Mapping;

public class TypeConvertingCSharpMapping : CSharpMappingBase
{
    [Obsolete]
    public TypeConvertingCSharpMapping(MappingModel model, ICSharpFileBuilderTemplate template) : this(model, (ICSharpTemplate)template)
    {
    }

    public TypeConvertingCSharpMapping(MappingModel model, ICSharpTemplate template) : base(model, template)
    {
    }

    // Only apply the type conversions on assignments, not on GetSourceStatement():
    // This is so that source statements used in null checks or queries don't get converted.
    public override IEnumerable<CSharpStatement> GetMappingStatements()
    {
        yield return new CSharpAssignmentStatement(GetTargetStatement(), GetTypeConvertedSourceStatement());
    }

    public CSharpStatement GetTypeConvertedSourceStatement()
    {
        if (!Mapping.IsOneToOne() || Mapping.TargetElement.TypeReference == null || Mapping.SourceElement.TypeReference == null)
        {
            return base.GetSourceStatement();
        }

        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.HasStringType() == true &&
            Mapping.SourceElement.TypeReference.HasStringType() == false)
        {
            return new CSharpAccessMemberStatement(base.GetSourceStatement(), new CSharpInvocationStatement("ToString").WithoutSemicolon());
        }

        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.HasGuidType() == true &&
            Mapping.SourceElement.TypeReference.IsNullable == false &&
            Mapping.SourceElement.TypeReference.HasStringType() == true)
        {
            return new CSharpAccessMemberStatement("Guid", new CSharpInvocationStatement("Parse").AddArgument(base.GetSourceStatement()).WithoutSemicolon());
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
            (Mapping.TargetElement.TypeReference.Element.IsTypeDefinitionModel() || Mapping.TargetElement.TypeReference.Element.IsEnumModel()) &&
            !Mapping.TargetElement.TypeReference.Element.IsStringType())
        {
            return new CSharpAccessMemberStatement(base.GetSourceStatement(), "Value");
        }
        return base.GetSourceStatement();
    }
}