using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;

namespace Intent.SoftwareFactory.Templates
{
    public class ClassTypeSource : IClassTypeSource
    {
        private Func<ITypeReference, string> _execute;

        internal ClassTypeSource(Func<ITypeReference, string> execute)
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

        public string GetClassType(ITypeReference typeInfo)
        {
            return _execute(typeInfo);
        }
    }
}
