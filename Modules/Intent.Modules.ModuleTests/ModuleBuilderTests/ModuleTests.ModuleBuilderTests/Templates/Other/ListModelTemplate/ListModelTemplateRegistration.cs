using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;
using Intent.Modelers.Domain.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Other.ListModelTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ListModelTemplateRegistration : ListModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metadataManager;

        public ListModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ListModelTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IClass> model)
        {
            return new ListModelTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<IClass> GetModels(IApplication application)
        {
            // Filter classes by SpecializationType if necessary (e.g. .Where(x => x.SpecializationType == "Service") for services only)
            return _metadataManager.GetDomainClasses(application.Id)
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}