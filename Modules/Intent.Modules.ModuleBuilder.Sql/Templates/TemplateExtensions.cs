using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Sql.Templates.SqlFileTemplatePartial;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Sql.Templates
{
    public static class TemplateExtensions
    {
        public static string GetSqlFileTemplatePartialName<T>(this IntentTemplateBase<T> template) where T : Intent.ModuleBuilder.Sql.Api.SqlTemplateModel
        {
            return template.GetTypeName(SqlFileTemplatePartialTemplate.TemplateId, template.Model);
        }

        public static string GetSqlFileTemplatePartialName(this IntentTemplateBase template, Intent.ModuleBuilder.Sql.Api.SqlTemplateModel model)
        {
            return template.GetTypeName(SqlFileTemplatePartialTemplate.TemplateId, model);
        }

    }
}