using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Sql.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Sql.Templates.SqlFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Sql.SqlCustom
{
    [IntentManaged(Mode.Fully)]
    partial class SqlCustomTemplate : SqlTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Sql.SqlCustomTemplate";

        [IntentManaged(Mode.Fully)]
        public SqlCustomTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new SqlFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"SqlCustom",
                relativeLocation: ""
            );
        }
    }
}