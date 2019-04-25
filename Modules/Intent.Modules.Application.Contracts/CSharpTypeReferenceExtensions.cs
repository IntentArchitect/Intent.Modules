using System;
using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.Templates;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Application.Contracts
{
    public static class CSharpTypeReferenceExtensions
    {
        [Obsolete("To be converted to Type system")] // JL: Is this even possible considering it's also trying to use based on a template identifier? See Intent.Modules.HttpServiceProxy.Templates.Proxy.WebApiClientServiceProxyTemplate for an example of an approach that might work.
        public static string GetQualifiedName<T>(this ITypeReference typeInfo, T template, string templateIdentifier = DTOTemplate.IDENTIFIER)
            where T: IProjectItemTemplate, IRequireTypeResolver
        {
            var result = typeInfo.Name;
            if (typeInfo.Type == ReferenceType.ClassType)
            {
                var templateInstance = template.Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<DTOModel>(templateIdentifier, x => x.Id == typeInfo.Id));
                if (templateInstance != null)
                {
                    return $"{templateInstance.Namespace}.{templateInstance.ClassName}";
                }
            }
            else if (typeInfo.Stereotypes.Any(x => x.Name == StandardStereotypes.CSharpType))
            {
                return typeInfo.GetStereotypeProperty<string>(StandardStereotypes.CSharpType, "TypeName") + (typeInfo.IsNullable && typeInfo.GetStereotypeProperty(StandardStereotypes.CSharpType, "IsPrimitive", false) ? "?" : "");
            }
            else if (typeInfo.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") != null)
            {
                return $"{typeInfo.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace")}.{typeInfo.Name}";
            }
            else if (typeInfo.Folder?.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") != null)
            {
                return $"{typeInfo.Folder.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace")}.{typeInfo.Name}";
            }

            return template.Types.Get(typeInfo);
        }
    }
}