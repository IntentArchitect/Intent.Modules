using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Sql.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Sql.Templates.SqlFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Sql.SqlFilePerModel
{
    [IntentManaged(Mode.Fully)]
    partial class SqlFilePerModelTemplate : SqlTemplateBase<Intent.Modelers.Domain.Api.ClassModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Sql.SqlFilePerModelTemplate";

        [IntentManaged(Mode.Fully)]
        public SqlFilePerModelTemplate(IOutputTarget outputTarget, Intent.Modelers.Domain.Api.ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new SqlFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}",
                relativeLocation: "SqlFilePerModel"
            );
        }
    }
}