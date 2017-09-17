using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.Modules.Constants;
using Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts
{
    public static class TypescriptTypeReferenceExtensions
    {
        public static string ConvertType(this IProjectItemTemplate template, ITypeReference typeInfo)
        {
            var result = template.GetQualifiedName(typeInfo);
            if (typeInfo.IsCollection)
            {
                result = "" + result + "[]";
            }
            return result;
        }

        public static string GetQualifiedName(this IProjectItemTemplate template, ITypeReference typeInfo)
        {
            string result = typeInfo.Name;
            if (typeInfo.Type == ReferenceType.ClassType)
            {
                var templateInstance = template.Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<DTOModel>(TypescriptDtoTemplate.LocalIdentifier, (x) => x.Id == typeInfo.Id))
                    ?? template.Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<DTOModel>(TypescriptDtoTemplate.RemoteIdentifier, (x) => x.Id == typeInfo.Id));
                if (templateInstance != null)
                {
                    result = $"{templateInstance.Namespace}.{templateInstance.ClassName}";
                }
            }
            else if (typeInfo.HasStereotype(StandardStereotypes.TypescriptType))
            {
                result = typeInfo.Stereotypes.GetPropertyValue<string>(StandardStereotypes.TypescriptType, "TypeName");
            }

            return result;
        }
    }
}