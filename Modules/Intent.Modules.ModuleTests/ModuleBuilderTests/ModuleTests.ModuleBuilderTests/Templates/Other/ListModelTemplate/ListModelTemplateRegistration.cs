using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Other.ListModelTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ListModelTemplateRegistration : ListModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ListModelTemplateRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => ListModelTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IClass> model)
        {
            return new ListModelTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            // Filter classes by SpecializationType if necessary (e.g. .Where(x => x.SpecializationType == "Service") for services only)
            return _metaDataManager.GetClassModels(application, "Domain")
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}