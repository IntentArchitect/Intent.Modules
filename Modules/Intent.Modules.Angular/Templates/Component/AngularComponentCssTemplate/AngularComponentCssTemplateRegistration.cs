using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;
using System;
using Intent.Modules.Angular.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Component.AngularComponentCssTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AngularComponentCssTemplateRegistration : ModelTemplateRegistrationBase<IComponentModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public AngularComponentCssTemplateRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => Component.AngularComponentCssTemplate.AngularComponentCssTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IComponentModel model)
        {
            return new Component.AngularComponentCssTemplate.AngularComponentCssTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IComponentModel> GetModels(Engine.IApplication application)
        {
            return _metaDataManager.GetModules(application).SelectMany(x => x.Components);
        }
    }
}