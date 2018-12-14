using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial
{
    public class RoslynProjectItemTemplatePartialRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public RoslynProjectItemTemplatePartialRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new RoslynProjectItemTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsCSharpTemplate())
                .ToList();
        }
    }
}