using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.CSharp
{
    public class CSharpTypeSource : ITypeSource
    {
        private readonly Func<ITypeReference, CSharpTypeSource, IResolvedTypeInfo> _execute;
        private readonly IList<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();

        internal CSharpTypeSource(Func<ITypeReference, CSharpTypeSource, IResolvedTypeInfo> execute)
        {
            _execute = execute;
        }

        public static ITypeSource Create(ISoftwareFactoryExecutionContext context, string templateId, string collectionFormat = "IEnumerable<{0}>")
        {
            return new CSharpTypeSource((typeInfo, _this) =>
            {
                var typeName = _this.GetTypeName(context, templateId, typeInfo, collectionFormat);
                return typeName;
            });
        }

        private IResolvedTypeInfo GetTypeName(ISoftwareFactoryExecutionContext context, string templateId, ITypeReference typeInfo, string collectionFormat)
        {
            var templateInstance = GetTemplateInstance(context, templateId, typeInfo);

            if (templateInstance == null)
            {
                return null;
            }
            var name = templateInstance.FullTypeName() + (typeInfo.GenericTypeParameters.Any() 
                    ? $"<{string.Join(", ", typeInfo.GenericTypeParameters.Select(x => GetTypeName(context, templateId, x, collectionFormat)))}>" 
                    : "");

            if (!string.IsNullOrWhiteSpace(name) && typeInfo.IsCollection)
            {
                name = string.Format(collectionFormat, name);
            }

            return new ResolvedTypeInfo(name, false, templateInstance);
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            return _execute(typeInfo, this);
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _templateDependencies;
        }

        private IClassProvider GetTemplateInstance(ISoftwareFactoryExecutionContext context, string templateId, ITypeReference typeInfo)
        {
            var templateInstance = context.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel<IMetadataModel>(templateId, (x) => x.Id == typeInfo.Element.Id, $"Model Id: {typeInfo.Element.Id}"));
            if (templateInstance != null)
            {
                _templateDependencies.Add(TemplateDependency.OnModel<IMetadataModel>(templateId, (x) => x.Id == typeInfo.Element.Id, $"Model Id: {typeInfo.Element.Id}"));
            }

            return templateInstance;
        }
    }
}
