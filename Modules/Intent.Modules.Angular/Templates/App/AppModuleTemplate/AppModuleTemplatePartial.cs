using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates.Core.CoreModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate;
using Intent.Modules.Common.TypeScript.Editor;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.App.AppModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class AppModuleTemplate : TypeScriptTemplateBase<object>, IHasNugetDependencies
    {
        private readonly IList<string> _components = new List<string>() { "AppComponent"};
        private readonly IList<string> _providers = new List<string>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.App.AppModuleTemplate";

        public AppModuleTemplate(IProject project, object model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularComponentCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularComponentCreatedEvent.ModuleId) != ClassName)
                {
                    return;
                }

                _components.Add(GetTemplateClassName(@event.GetValue(AngularComponentCreatedEvent.ModelId)));
            });

            project.Application.EventDispatcher.Subscribe(AngularServiceProxyCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularServiceProxyCreatedEvent.ModuleId) != ClassName)
                {
                    return;
                }

                var template = GetTemplateClassName(@event.GetValue(AngularServiceProxyCreatedEvent.ModelId));
                _providers.Add(template);
            });
        }

        public string AppRoutingModuleClassName => GetTemplateClassName(AppRoutingModuleTemplate.AppRoutingModuleTemplate.TemplateId);
        public string CoreModule => GetTemplateClassName(CoreModuleTemplate.TemplateId);

        public string GetComponents()
        {
            if (!_components.Any())
            {
                return "";
            }
            return $"{System.Environment.NewLine}    " + string.Join($",{System.Environment.NewLine}    ", _components) + $"{System.Environment.NewLine}  ";
        }

        public string GetProviders()
        {
            if (!_providers.Any())
            {
                return "";
            }
            return $"{System.Environment.NewLine}    " + string.Join($",{System.Environment.NewLine}    ", _providers) + $"{System.Environment.NewLine}  ";
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypeScriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"app.module",
                relativeLocation: $"ClientApp/src/app",
                className: "AppModule"
            );
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            // Reason for this version:
            // Angular 8 wants Typescript >= 3.4.0 and < 3.6.0, but Visual Studio 2019 builds using 3.7.
            // https://stackoverflow.com/questions/58485673/vs2019-error-ts2300-duplicate-identifier-iteratorresult
            var packages = new List<INugetPackageInfo>()
            {
                new NugetPackageInfo("Microsoft.TypeScript.MsBuild", "3.5.3")
            };

            if (Project.IsNetCore3App())
            {
                packages.Add(new NugetPackageInfo("Microsoft.AspNetCore.SpaServices.Extensions", "3.1.4"));
            }

            return packages;
        }
    }
}