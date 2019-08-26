using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.ModuleBuilder.Helpers;

namespace Intent.Modules.ModuleBuilder.Templates.Decorators
{
    public class DecoratorRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        public override string TemplateId => DecoratorTemplate.TemplateId;

        private readonly IMetaDataManager _metaDataManager;

        public DecoratorRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DecoratorTemplate(project, model);
        }

        public override IEnumerable<IClass> GetModels(SoftwareFactory.Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsDecoratorTemplate())
                .ToList();
        }
    }
}
