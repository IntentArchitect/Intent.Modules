using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace ModuleBuilders.Templates.Sql.SqlCustom
{
    [IntentManaged(Mode.Fully)]
    public class SqlCustomTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public SqlCustomTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => SqlCustomTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            registry.RegisterTemplate(TemplateId, project => new SqlCustomTemplate(project, null));
        }
    }
}