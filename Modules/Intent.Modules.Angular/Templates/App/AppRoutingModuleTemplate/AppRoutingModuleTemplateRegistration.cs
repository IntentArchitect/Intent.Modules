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

namespace Intent.Modules.Angular.Templates.App.AppRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AppRoutingModuleTemplateRegistration : SingleFileListModelTemplateRegistration<ModuleModel>
    {
        public override string TemplateId => App.AppRoutingModuleTemplate.AppRoutingModuleTemplate.TemplateId;

        private readonly IMetadataManager _metadataManager;

        public AppRoutingModuleTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget project, IList<ModuleModel> model)
        {
            return new App.AppRoutingModuleTemplate.AppRoutingModuleTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ModuleModel> GetModels(IApplication application)
        {
            return _metadataManager.Angular(application).GetModuleModels().ToList();
        }
    }
}