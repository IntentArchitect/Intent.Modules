using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using System;
using Intent.Modules.Angular.Templates.Shared.IntentDecoratorsTemplate;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.App.AppRoutingModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AppRoutingModuleTemplate : TypeScriptTemplateBase<IList<ModuleModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.App.AppRoutingModuleTemplate.AppRoutingModuleTemplate";

        public AppRoutingModuleTemplate(IOutputTarget project, IList<ModuleModel> model) : base(TemplateId, project, model)
        {
            AddTemplateDependency(IntentDecoratorsTemplate.TemplateId);
        }

        public IEnumerable<AngularModuleTemplate> ModuleTemplates => Model.Select(x => GetTemplate<AngularModuleTemplate>(TemplateDependency.OnModel(AngularModuleTemplate.TemplateId, x)));

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"app-routing.module",
                relativeLocation: $"",
                className: "AppRoutingModule"
            );
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var module in Model)
            {
                ExecutionContext.EventDispatcher.Publish(AngularModuleRouteCreatedEvent.EventId, new Dictionary<string, string>()
                {
                    {AngularModuleRouteCreatedEvent.ModuleName, module.GetModuleName()},
                    {AngularModuleRouteCreatedEvent.Route, GetRoute(module)},
                });
            }
        }

        private string GetRoute(ModuleModel module)
        {
            return module.Name.Replace("Module", "").ToLower();
        }
    }
}