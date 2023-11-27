using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace ModuleBuilders.Templates.File.IndentedFileBuilderFilePerModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IndentedFileBuilderFilePerModelTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public IndentedFileBuilderFilePerModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => IndentedFileBuilderFilePerModelTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new IndentedFileBuilderFilePerModelTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels();
        }
    }
}