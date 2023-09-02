using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Templates.Templates.IatSpecFile
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IatSpecFileTemplateRegistration : FilePerModelTemplateRegistration<ApplicationTemplateModel>
    {
        public override string TemplateId => IatSpecFileTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ApplicationTemplateModel model)
        {
            return new IatSpecFileTemplate(outputTarget, model);
        }
        private readonly IMetadataManager _metadataManager;

        public IatSpecFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ApplicationTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.AppTemplates(application).GetApplicationTemplateModels();
        }
    }
}