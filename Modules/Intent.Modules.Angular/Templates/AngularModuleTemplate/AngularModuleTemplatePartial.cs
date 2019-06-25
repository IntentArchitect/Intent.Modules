using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Templates.Component.AngularComponentCssTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate;
using Intent.Modules.Common.Plugins;
using Intent.Templates;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.AngularModuleTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularModuleTemplate : IntentProjectItemTemplateBase<IModuleModel>
    {
        public const string TemplateId = "Angular.AngularModuleTemplate";
        private readonly IList<AngularComponentInfo> _components = new List<AngularComponentInfo>();
        private readonly IList<AngularProviderInfo> _providers = new List<AngularProviderInfo>();

        public AngularModuleTemplate(IProject project, IModuleModel model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularComponentCreatedEvent.EventId, @event =>
                {
                    if (@event.GetValue(AngularComponentCreatedEvent.ModuleId) != Model.Id)
                    {
                        return;
                    }

                    var template = Project.FindTemplateInstance<ITemplate>(TemplateDependency.OnModel<IMetadataModel>(AngularComponentTsTemplate.TemplateId, x => x.Id == @event.GetValue(AngularComponentCreatedEvent.ModelId)));
                    _components.Add(new AngularComponentInfo(((IHasClassDetails)template).ClassName, template.GetMetadata().GetRelativeFilePathWithFileName()));
                });

            project.Application.EventDispatcher.Subscribe(AngularServiceProxyCreatedEvent.EventId, @event =>
            {
                if (@event.GetValue(AngularServiceProxyCreatedEvent.ModuleId) != Model.Id)
                {
                    return;
                }

                var template = Project.FindTemplateInstance<ITemplate>(TemplateDependency.OnModel<IMetadataModel>(AngularServiceProxyTemplate.TemplateId, x => x.Id == @event.GetValue(AngularServiceProxyCreatedEvent.ModelId)));
                _providers.Add(new AngularProviderInfo(((IHasClassDetails)template).ClassName, template.GetMetadata().GetRelativeFilePathWithFileName()));
            });
        }

        public string ModuleName => Model.GetModuleName();

        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var editor = new TypescriptFileEditor(source);
            foreach (var componentInfo in _components)
            {
                editor.AddImportIfNotExists(componentInfo.ComponentName, GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(componentInfo.Location));
                editor.AddDeclarationIfNotExists(componentInfo.ComponentName);
            }

            foreach (var providerInfo in _providers)
            {
                editor.AddImportIfNotExists(providerInfo.ProviderName, GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(providerInfo.Location));
                editor.AddProviderIfNotExists(providerInfo.ProviderName);
            }

            return editor.GetSource();
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : TransformText();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ModuleName.ToAngularFileName()}.module",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client\\src\\app\\{ ModuleName.ToAngularFileName() }"
            );
        }

        private string GetComponentName(IComponentModel componentModel)
        {
            var componentTemplate = Project.FindTemplateInstance<Component.AngularComponentTsTemplate.AngularComponentTsTemplate>(Component.AngularComponentTsTemplate.AngularComponentTsTemplate.TemplateId, componentModel);
            return componentTemplate.ComponentName;
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

    public static class IModuleModelExtensions
    {
        public static string GetModuleName(this IModuleModel module)
        {
            if (module.Name.EndsWith("Module", StringComparison.InvariantCultureIgnoreCase))
            {
                return module.Name.Substring(0, module.Name.Length - "Module".Length);
            }
            return module.Name;
        }
    }
}