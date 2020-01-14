using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Angular.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.AppRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AppRoutingModuleTemplateRegistration : ListModelTemplateRegistrationBase<IModuleModel>
    {
        public override string TemplateId => AppRoutingModuleTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IModuleModel> model)
        {
            return new AppRoutingModuleTemplate(project, model);
        }
        private readonly IMetadataManager _metadataManager;

        public AppRoutingModuleTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<IModuleModel> GetModels(IApplication application)
        {
            return _metadataManager.GetModules(application).ToList();
        }
    }
}