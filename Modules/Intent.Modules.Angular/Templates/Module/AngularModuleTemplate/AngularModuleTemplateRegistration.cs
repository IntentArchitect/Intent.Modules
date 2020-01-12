using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Module.AngularModuleTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AngularModuleTemplateRegistration : ModelTemplateRegistrationBase<IModuleModel>
    {
        private readonly IMetadataManager _metadataManager;

        public AngularModuleTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => Module.AngularModuleTemplate.AngularModuleTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IModuleModel model)
        {
            return new Module.AngularModuleTemplate.AngularModuleTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IModuleModel> GetModels(IApplication application)
        {
            return _metadataManager.GetModules(application);
        }
    }
}