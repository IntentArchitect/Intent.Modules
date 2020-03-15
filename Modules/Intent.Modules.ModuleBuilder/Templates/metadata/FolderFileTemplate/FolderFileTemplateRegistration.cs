using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.metadata.FolderFileTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class FolderFileTemplateRegistration : ModelTemplateRegistrationBase<IFolder>
    {
        private readonly IMetadataManager _metadataManager;

        public FolderFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => FolderFileTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IFolder model)
        {
            return new FolderFileTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IFolder> GetModels(IApplication application)
        {
            return new IFolder[0];
            //return _metadataManager.Get(application);
        }
    }
}