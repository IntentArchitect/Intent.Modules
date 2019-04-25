using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates
using System;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Component.AngularComponentHtmlTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AngularComponentHtmlTemplateRegistration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metaDataManager;

        public AngularComponentHtmlTemplateRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => Component.AngularComponentHtmlTemplate.AngularComponentHtmlTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new Component.AngularComponentHtmlTemplate.AngularComponentHtmlTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IClass> GetModels(Engine.IApplication application)
        {
            return _metaDataManager.GetClassModels(application, "Angular").Where(x => x.SpecializationType == "Component");
        }
    }
}