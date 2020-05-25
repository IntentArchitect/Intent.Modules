using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Editor;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Module.AngularRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AngularRoutingModuleTemplate : TypeScriptTemplateBase<ModuleModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Module.AngularRoutingModuleTemplate";

        public AngularRoutingModuleTemplate(IProject project, ModuleModel model) : base(TemplateId, project, model, TypescriptTemplateMode.UpdateFile)
        {
        }

        public string ModuleName => Model.GetModuleName() + "Routing";

        protected override void ApplyFileChanges(TypeScriptFile file)
        {
            var routes = file.VariableDeclarations().FirstOrDefault();
            foreach (var component in Model.Components)
            {
                var routeExists = routes.GetAssignedValue<TypeScriptArrayLiteralExpression>()
                    .GetValues<TypeScriptObjectLiteralExpression>()
                    .Any(x => x.PropertyAssignmentExists("component", GetClassName(component)));
                if (!routeExists)
                {
                    var array = routes.GetAssignedValue<TypeScriptArrayLiteralExpression>();
                    array.AddValue($@"{{
    path: '{GetPath(component)}',
    component: {GetClassName(component)}  #>
  }}");
                }
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ModuleName.ToKebabCase()}.module",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"ClientApp/src/app/{ Model.GetModuleName().ToKebabCase() }",
                className: "${ModuleName}"
            );
        }

        private string GetPath(ComponentModel component)
        {
            return GetTemplateClassName(AngularComponentTsTemplate.TemplateId, component).Replace("Component", "").ToKebabCase();
        }

        private string GetClassName(ComponentModel component)
        {
            return GetTemplateClassName(AngularComponentTsTemplate.TemplateId, component);
        }
    }
}