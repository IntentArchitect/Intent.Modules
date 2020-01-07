using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Other.CustomCreationMode
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CustomCreationModeRegistration : IProjectTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public CustomCreationModeRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => CustomCreationMode.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new CustomCreationMode(project, null));
        }
    }
}