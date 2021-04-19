using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.ModuleBuilder.Sql.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Sql.Templates.SqlFileTemplatePartial
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class SqlFileTemplatePartialTemplate : CSharpTemplateBase<SqlTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Sql.Templates.SqlFileTemplatePartial";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public SqlFileTemplatePartialTemplate(IOutputTarget outputTarget, SqlTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentCommonSql);
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        //public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        //public string FolderPath => string.Join("/", OutputFolder);
        //public string FolderNamespace => string.Join(".", OutputFolder);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.Name)}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.Name)}",
                fileName: $"{TemplateName}Partial");
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(
                modelId: Model.Id,
                templateId: GetTemplateId(),
                templateType: "Sql Template",
                role: GetRole(),
                location: Model.GetLocation()));

            Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: "Intent.Common.Sql",
                moduleVersion: "3.0.3"));
            if (Model.GetModelType() != null)
            {
                Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private string GetRole()
        {
            return Model.GetRole();
        }

        public string GetTemplateId()
        {
            return $"{Model.GetModule().Name}.{string.Join(".", Model.GetParentFolderNames().Concat(new[] { Model.Name }))}";
        }

        private string GetModelType()
        {
            return Model.GetModelName();
        }
    }
}