using System;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;

namespace Intent.SoftwareFactory.Templates
{
    public class ClassTypeSource : IClassTypeSource
    {
        private Func<MetaModel.Common.ITypeReference, string> _execute;

        internal ClassTypeSource(Func<MetaModel.Common.ITypeReference, string> execute)
        {
            _execute = execute;
        }

        public static IClassTypeSource InProject(IProject project, string templateId)
        {
            return new ClassTypeSource((typeInfo) => project.FindTemplateInstance<IHasClassDetails>(
                TemplateDependancy.OnModel<IMetaModel>(templateId, (x) => x.Id == typeInfo.Id)
                )?.FullTypeName());
        }

        public static IClassTypeSource InApplication(IApplication application, string templateId)
        {
            return new ClassTypeSource((typeInfo) => application.FindTemplateInstance<IHasClassDetails>(
                TemplateDependancy.OnModel<IMetaModel>(templateId, (x) => x.Id == typeInfo.Id)
                )?.FullTypeName());
        }

        public string GetClassType(MetaModel.Common.ITypeReference typeInfo)
        {
            return _execute(typeInfo);
        }
    }
}
