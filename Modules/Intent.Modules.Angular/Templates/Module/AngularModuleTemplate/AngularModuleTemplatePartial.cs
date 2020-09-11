using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Module.AngularModuleTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AngularModuleTemplate : TypeScriptTemplateBase<ModuleModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Module.AngularModuleTemplate";
        private readonly IList<ITemplate> _components = new List<ITemplate>();
        private readonly IList<ITemplate> _providers = new List<ITemplate>();

        public AngularModuleTemplate(IProject project, ModuleModel model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularComponentCreatedEvent.EventId, @event =>
                {
                    if (@event.GetValue(AngularComponentCreatedEvent.ModuleId) != Model.Id)
                    {
                        return;
                    }

                    _components.Add(FindTemplate<ITemplate>(AngularComponentTsTemplate.TemplateId, @event.GetValue(AngularComponentCreatedEvent.ModelId)));
                });

            project.Application.EventDispatcher.Subscribe(AngularServiceProxyCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularServiceProxyCreatedEvent.ModuleId) != Model.Id)
                {
                    return;
                }

                //_components.Add(FindTemplate<ITemplate>(AngularServiceProxyTemplate.TemplateId, @event.GetValue(AngularServiceProxyCreatedEvent.ModelId)));
                var template = FindTemplate<ITemplate>(AngularServiceProxyTemplate.TemplateId, @event.GetValue(AngularServiceProxyCreatedEvent.ModelId));
                _providers.Add(template);
            });
        }

        public string ModuleName => Model.GetModuleName();

        public string RoutingModuleClassName => GetTemplateClassName(AngularRoutingModuleTemplate.AngularRoutingModuleTemplate.TemplateId, Model);

        //protected override void ApplyFileChanges(TypescriptFile file)
        //{
        //    foreach (var template in _components)
        //    {
        //        var ngModuleDecorator = file.ClassDeclarations().First().Decorators().FirstOrDefault(x => x.Name == "NgModule")?.ToNgModule();
        //        ngModuleDecorator?.AddDeclarationIfNotExists(GetTemplateClassName(template));
        //        file.UpdateChanges();
        //    }

        //    foreach (var template in _providers)
        //    {
        //        var ngModuleDecorator = file.ClassDeclarations().First().Decorators().FirstOrDefault(x => x.Name == "NgModule")?.ToNgModule();
        //        ngModuleDecorator?.AddProviderIfNotExists(GetTemplateClassName(template));
        //        file.UpdateChanges();
        //    }
        //}
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
            return $"{System.Environment.NewLine}    " + string.Join($",{System.Environment.NewLine}    ", _components.Select(GetTemplateClassName)) + $"{System.Environment.NewLine}  ";
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
            return $"{System.Environment.NewLine}    " + string.Join($",{System.Environment.NewLine}    ", _providers.Select(GetTemplateClassName)) + $"{System.Environment.NewLine}  ";
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypeScriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{ModuleName.ToKebabCase()}.module",
                relativeLocation: $"ClientApp/src/app/{ ModuleName.ToKebabCase() }",
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
    }
}