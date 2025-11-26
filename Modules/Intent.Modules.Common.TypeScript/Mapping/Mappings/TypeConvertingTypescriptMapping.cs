using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;
using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.Typescript.Mapping;

public class TypeConvertingTypescriptMapping : TypescriptMappingBase
{
    [Obsolete]
    public TypeConvertingTypescriptMapping(MappingModel model, ITypescriptFileBuilderTemplate template) : this(model, (ITypescriptTemplate)template)
    {
    }

    public TypeConvertingTypescriptMapping(MappingModel model, ITypescriptTemplate template) : base(model, template)
    {
    }

    // Only apply the type conversions on assignments, not on GetSourceStatement():
    // This is so that source statements used in null checks or queries don't get converted.
    public override IEnumerable<TypescriptStatement> GetMappingStatements()
    {
        yield return new TypescriptStatement($"{GetTargetStatement()}: {GetTypeConvertedSourceStatement()}");
    }

    public TypescriptStatement GetTypeConvertedSourceStatement()
    {
        if (!Mapping.IsOneToOne() || Mapping.TargetElement.TypeReference == null || Mapping.SourceElement.TypeReference == null)
        {
            return base.GetSourceStatement();
        }

        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.HasStringType() == true &&
            Mapping.SourceElement.TypeReference.HasStringType() == false)
        {
            // TODO
            return new TypescriptStatement($"this.{base.GetSourceStatement()}");
        }

        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.HasGuidType() == true &&
            Mapping.SourceElement.TypeReference.IsNullable == false &&
            Mapping.SourceElement.TypeReference.HasStringType() == true)
        {
            // TODO
            return new TypescriptStatement("");
            //return new CSharpAccessMemberStatement("Guid", new CSharpInvocationStatement("Parse").AddArgument(base.GetSourceStatement()).WithoutSemicolon());
        }
        if (Mapping.IsOneToOne() &&
            Mapping.TargetElement.TypeReference.Element?.SpecializationType == "Enum" &&
            Mapping.SourceElement.TypeReference.HasIntType())
        {
            // TODO
            return new TypescriptStatement("");
            //return $"({Template.GetTypeName((IElement)Mapping.TargetElement.TypeReference.Element)}){base.GetSourceStatement()}";
        }

        if (Mapping.IsOneToOne() &&
            Mapping.SourceElement.TypeReference.IsNullable &&
            !Mapping.TargetElement.TypeReference.IsNullable &&
            (Mapping.TargetElement.TypeReference.Element.IsTypeDefinitionModel() || Mapping.TargetElement.TypeReference.Element.IsEnumModel()) &&
            !Mapping.TargetElement.TypeReference.Element.IsStringType())
        {
            return base.GetSourceStatement();
        }

        return base.GetSourceStatement();
    }
}