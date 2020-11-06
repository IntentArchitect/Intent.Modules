using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate;
using Intent.Modules.Angular.Templates.Shared.IntentDecoratorsTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Module.AngularModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AngularModuleTemplate : TypeScriptTemplateBase<ModuleModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Module.AngularModuleTemplate.AngularModuleTemplate";
        private readonly ISet<string> _components = new HashSet<string>();
        private readonly ISet<string> _providers = new HashSet<string>();
        private readonly ISet<string> _angularImports = new HashSet<string>();
        private readonly ISet<string> _imports = new HashSet<string>();

        public AngularModuleTemplate(IOutputTarget project, ModuleModel model) : base(TemplateId, project, model)
        {
            AddTemplateDependency(IntentDecoratorsTemplate.TemplateId);
            project.Application.EventDispatcher.Subscribe(AngularComponentCreatedEvent.EventId, @event =>
                {
                    if (@event.GetValue(AngularComponentCreatedEvent.ModuleId) != Model.Id)
                    {
                        return;
                    }

                    _components.Add(GetTypeName(AngularComponentTsTemplate.TemplateId, @event.GetValue(AngularComponentCreatedEvent.ModelId)));
                });

            project.Application.EventDispatcher.Subscribe(AngularServiceProxyCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularServiceProxyCreatedEvent.ModuleId) != Model.Id)
                {
                    return;
                }

                var templateClassName = GetTypeName(AngularServiceProxyTemplate.TemplateId, @event.GetValue(AngularServiceProxyCreatedEvent.ModelId));
                _providers.Add(templateClassName);
            });

            project.Application.EventDispatcher.Subscribe(AngularImportDependencyRequiredEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularImportDependencyRequiredEvent.ModuleId) != Model.Id)
                {
                    return;
                }

                _angularImports.Add(@event.GetValue(AngularImportDependencyRequiredEvent.Dependency));
                _imports.Add(@event.GetValue(AngularImportDependencyRequiredEvent.Import));
            });
        }

        public string ModuleName => Model.GetModuleName();

        public string RoutingModuleClassName => GetTypeName(AngularRoutingModuleTemplate.AngularRoutingModuleTemplate.TemplateId, Model);

        public string GetImports()
        {
            if (!_imports.Any())
            {
                return "";
            }
            return $"{System.Environment.NewLine}" + string.Join($"{System.Environment.NewLine}", _imports);
        }

        public bool HasComponents()
        {
            return _components.Any();
        }

        public string GetComponents()
        {
            if (!_components.Any())
            {
                return "";
            }
            return $"{System.Environment.NewLine}    " + string.Join($",{System.Environment.NewLine}    ", _components) + $"{System.Environment.NewLine}  ";
        }

        public bool HasProviders()
        {
            return _providers.Any();
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
                fileName: $"{ModuleName.ToKebabCase()}.module",
                relativeLocation: $"{ ModuleName.ToKebabCase() }",
                className: "${ModuleName}Module");
        }
    }

    internal class AngularComponentInfo
    {
        public AngularComponentInfo(string componentName, string location)
        {
            ComponentName = componentName;
            Location = location;
        }

        public string ComponentName { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return $"Component: {ComponentName} - {Location}";
        }
    }

    internal class AngularProviderInfo
    {
        public AngularProviderInfo(string providerName, string location)
        {
            ProviderName = providerName;
            Location = location;
        }

        public string ProviderName { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return $"Provider: {ProviderName} - {Location}";
        }
    }
}