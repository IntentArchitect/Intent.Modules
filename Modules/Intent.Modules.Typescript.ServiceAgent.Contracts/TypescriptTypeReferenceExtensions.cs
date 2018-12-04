using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts
{
    public static class TypescriptTypeReferenceExtensions
    {
        public static string ConvertType<T>(this T template, ITypeReference typeInfo)
            where T : IProjectItemTemplate, IRequireTypeResolver
        {
            var result = template.GetQualifiedName(typeInfo);
            if (typeInfo.IsCollection)
            {
                result = "" + result + "[]";
            }
            return result;
        }

        public static string GetQualifiedName<T>(this T template, ITypeReference typeInfo)
            where T : IProjectItemTemplate, IRequireTypeResolver
        {
            string result = typeInfo.Name;
            if (typeInfo.Type == ReferenceType.ClassType)
            {
                var templateInstance = template.Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<DTOModel>(TypescriptDtoTemplate.LocalIdentifier, (x) => x.Id == typeInfo.Id))
                    ?? template.Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<DTOModel>(TypescriptDtoTemplate.RemoteIdentifier, (x) => x.Id == typeInfo.Id));
                if (templateInstance != null)
                {
                    return $"{templateInstance.Namespace}.{templateInstance.ClassName}";
                }
            }
            else if (typeInfo.HasStereotype(StandardStereotypes.TypescriptType))
            {
                return typeInfo.GetStereotypeProperty<string>(StandardStereotypes.TypescriptType, "TypeName");
            }

            return template.Types.Get(typeInfo);
        }
    }
}