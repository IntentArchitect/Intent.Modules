using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Sql.Api;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Sql.Templates.SqlFileTemplatePreProcessedFile
{
    public class SqlFileTemplatePreProcessedFileRegistrations : FilePerModelTemplateRegistration<SqlTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SqlFileTemplatePreProcessedFileRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "ModuleBuilder.Sql.Templates.SqlFileTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IOutputTarget project, SqlTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: SqlFileTemplate.SqlFileTemplate.TemplateId,
                partialTemplateId: SqlFileTemplatePartial.SqlFileTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<SqlTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetSqlTemplateModels();
        }
    }
}
