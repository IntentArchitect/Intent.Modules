using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Constants;
using Intent.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts
{
    public static class TypescriptTypeReferenceExtensions
    {
        //public static string ConvertType<T>(this T template, ITypeReference typeInfo)
        //    where T : ITemplate, IRequireTypeResolver
        //{
        //    var result = template.GetQualifiedName(typeInfo);
        //    return result;
        //}

        //[Obsolete("Should use Types.Get(...) system")]
        //public static string GetQualifiedName<T>(this T template, ITypeReference typeInfo)
        //    where T : ITemplate, IRequireTypeResolver
        //{
        //    string result = typeInfo.Element.Name;
        //    if (typeInfo.Element.SpecializationType == "DTO")
        //    {
        //        var templateInstance = template.OutputTarget.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<DTOModel>(TypescriptDtoTemplate.LocalIdentifier, (x) => x.Id == typeInfo.Element.Id))
        //            ?? template.OutputTarget.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<DTOModel>(TypescriptDtoTemplate.RemoteIdentifier, (x) => x.Id == typeInfo.Element.Id));
        //        if (templateInstance != null)
        //        {
        //            return $"{templateInstance.Namespace}.{templateInstance.ClassName}";
        //        }
        //    }
        //    else if (typeInfo.Element.HasStereotype(StandardStereotypes.TypescriptType))
        //    {
        //        return typeInfo.Element.GetStereotypeProperty<string>(StandardStereotypes.TypescriptType, "TypeName");
        //    }

        //    return template.Types.Get(typeInfo);
        //}
    }
}