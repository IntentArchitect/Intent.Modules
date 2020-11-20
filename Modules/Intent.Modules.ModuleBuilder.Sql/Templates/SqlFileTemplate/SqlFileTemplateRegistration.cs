using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.ModuleBuilder.Sql.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Sql.Templates.SqlFileTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class SqlFileTemplateRegistration : FilePerModelTemplateRegistration<SqlTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SqlFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SqlFileTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, SqlTemplateModel model)
        {
            return new SqlFileTemplate(project, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<SqlTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetSqlTemplateModels();
        }
    }
}