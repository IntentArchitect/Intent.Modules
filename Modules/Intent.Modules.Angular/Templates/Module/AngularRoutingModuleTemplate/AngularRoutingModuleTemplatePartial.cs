using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Angular.Templates.Shared.IntentDecoratorsTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Module.AngularRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AngularRoutingModuleTemplate : TypeScriptTemplateBase<ModuleModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Module.AngularRoutingModuleTemplate";

        public AngularRoutingModuleTemplate(IProject project, ModuleModel model) : base(TemplateId, project, model)
        {
            AddTemplateDependency(IntentDecoratorsTemplate.TemplateId);
        }

        public string ModuleName => Model.GetModuleName() + "Routing";

        //      protected override void ApplyFileChanges(TypescriptFile file)
        //      {
        //          var routes = file.VariableDeclarations().FirstOrDefault();
        //          foreach (var component in Model.Components)
        //          {
        //              var routeExists = routes.GetAssignedValue<TypescriptArrayLiteralExpression>()
        //                  .GetValues<TypescriptObjectLiteralExpression>()
        //                  .Any(x => x.PropertyAssignmentExists("component", GetClassName(component)));
        //              if (!routeExists)
        //              {
        //                  var array = routes.GetAssignedValue<TypescriptArrayLiteralExpression>();
        //                  array.AddValue($@"{{
        //  path: '{GetPath(component)}',
        //  component: {GetClassName(component)}  #>
        //}}");
        //              }
        //          }
        //      }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypeScriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{ModuleName.ToKebabCase()}.module",
                relativeLocation: $"ClientApp/src/app/{ Model.GetModuleName().ToKebabCase() }",
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