using System.IO;
using System.Linq;
using Intent.Engine;
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
using Intent.Modules.Angular.Templates.Shared.IntentDecoratorsTemplate;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Angular.Api;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.App.AppModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class AppModuleTemplate : TypeScriptTemplateBase<object>, IHasNugetDependencies
    {
        private readonly ISet<string> _components = new HashSet<string>() { "AppComponent" };
        private readonly ISet<string> _providers = new HashSet<string>();
        private readonly ISet<string> _angularImports = new HashSet<string>();
        private readonly ISet<string> _imports = new HashSet<string>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.App.AppModuleTemplate.AppModuleTemplate";

        public AppModuleTemplate(IOutputTarget project, object model) : base(TemplateId, project, model)
        {
            AddTemplateDependency(IntentDecoratorsTemplate.TemplateId);
            project.Application.EventDispatcher.Subscribe(AngularComponentCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularComponentCreatedEvent.ModuleId) != ClassName)
                {
                    return;
                }

                _components.Add(GetTypeName(@event.GetValue(AngularComponentCreatedEvent.ModelId)));
            });

            project.Application.EventDispatcher.Subscribe(AngularServiceProxyCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularServiceProxyCreatedEvent.ModuleId) != ClassName)
                {
                    return;
                }

                var template = GetTypeName(@event.GetValue(AngularServiceProxyCreatedEvent.ModelId));
                _providers.Add(template);
            });

            project.Application.EventDispatcher.Subscribe(AngularImportDependencyRequiredEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularImportDependencyRequiredEvent.ModuleId) != ClassName)
                {
                    return;
                }

                _angularImports.Add(@event.GetValue(AngularImportDependencyRequiredEvent.Dependency));
                _imports.Add(@event.GetValue(AngularImportDependencyRequiredEvent.Import));
            });
        }

        public string AppRoutingModuleClassName => GetTypeName(AppRoutingModuleTemplate.AppRoutingModuleTemplate.TemplateId);
        public string CoreModule => GetTypeName(CoreModuleTemplate.TemplateId);

        public string GetImports()
        {
            if (!_imports.Any())
            {
                return "";
            }
            return $"{System.Environment.NewLine}" + string.Join($"{System.Environment.NewLine}", _imports);
        }

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

        public string GetAngularImports()
        {
            if (!_angularImports.Any())
            {
                return "";
            }
            return $",{System.Environment.NewLine}    " + string.Join($",    {System.Environment.NewLine}    ", _angularImports);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"app.module",
                relativeLocation: $"",
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

            if (OutputTarget.IsNetCore3App())
            {
                packages.Add(new NugetPackageInfo("Microsoft.AspNetCore.SpaServices.Extensions", "3.1.4"));
            }

            return packages;
        }
    }
}