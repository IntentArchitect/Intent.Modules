using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Other.CustomCreationMode
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CustomCreationModeRegistration : IProjectTemplateRegistration
    {
        private readonly IMetaDataManager _metaDataManager;

        public CustomCreationModeRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => CustomCreationMode.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registery, Intent.SoftwareFactory.Engine.IApplication applicationManager)
        {
            registery.Register(TemplateId, project => new CustomCreationMode(project, null));
        }
    }
}