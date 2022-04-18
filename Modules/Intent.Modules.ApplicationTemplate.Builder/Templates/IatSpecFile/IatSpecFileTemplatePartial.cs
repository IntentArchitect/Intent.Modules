using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.ApplicationTemplate;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Module;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Templates.IatSpecFile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class IatSpecFileTemplate : IntentTemplateBase<ApplicationTemplateModel>, IHasNugetDependencies
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ApplicationTemplate.Builder.Templates.IatSpecFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public IatSpecFileTemplate(IOutputTarget outputTarget, ApplicationTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public IIconModel Icon => Model.Icon ?? OutputTarget.Application.Icon;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"metadata",
                fileExtension: "iatspec",
                relativeLocation: Model.Name
            );
        }

        public override string TransformText()
        {
            var result = new ApplicationTemplatePersistable()
            {
                Id = Model.Name,
                Version = Model.Version,
                SupportedClientVersions = Model.SupportedClientVersions,
                DisplayName = Model.DisplayName,
                ShortDescription = Model.Description,
                Authors = Model.Authors,
                Priority = int.TryParse(Model.Priority, out var priority) ? priority : 0,
                Icon = new IconModelPersistable()
                {
                    Type = (IconType)Icon.Type,
                    Source = Icon.Source
                },
                DefaultName = Model.DefaultName,
                RelativeOutputLocation = Model.DefaultOutputLocation,
                ComponentGroups = Model.Groups.Select(g => new ApplicationTemplate_ComponentGroup
                {
                    Name = g.Name,
                    SelectionMode = GetSelectionMode(g),
                    Components = g.Components.Select(c => new ApplicationTemplate_Component()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Included = c.IncludeByDefault,
                        Required = c.IsRequired,
                        Description = c.Description,
                        Icon = new IconModelPersistable()
                        {
                            Type = (IconType)c.Icon.Type,
                            Source = c.Icon.Source
                        },
                        Modules = c.Modules.Select(m => new ApplicationTemplate_ComponentModule()
                        {
                            Id = m.Name,
                            Version = m.Version
                        }).ToList(),
                        Dependencies = c.GetComponentSettings().Dependencies().Select(d => new ApplicationTemplate_ComponentDependency()
                        {
                            Id = d.Id,
                            Name = d.Name
                        }).ToList()
                    }).ToList()
                }).ToList(),
                AdditionalSettings = Model.SettingsConfigurations.Select(g => new SettingsGroupPersistable()
                {
                    Id = g.Id,
                    Title = g.Name,
                    Settings = g.Fields.Select(f => new SettingFieldPersistable()
                    {
                        Id = f.Value ?? f.Name,
                        Title = f.Name,
                        IsRequired = f.GetSettingsFieldConfiguration().IsRequired(),
                        ControlType = GetControlType(f.GetSettingsFieldConfiguration().ControlType().AsEnum()),
                        DefaultValue = GetDefaultValue(f.GetSettingsFieldConfiguration()),
                        Hint = f.GetSettingsFieldConfiguration().Hint(),
                        Options = f.Options.Select(o => new SettingFieldOptionPersistable()
                        {
                            Value = o.Value ?? o.Name,
                            Description = o.Name
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            if (result.AdditionalSettings.SelectMany(x => x.Settings).GroupBy(x => x.Id.ToLower()).Any(x => x.Count() > 1))
            {
                var duplicateIds = result.AdditionalSettings.SelectMany(x => x.Settings).GroupBy(x => x.Id.ToLower()).Where(x => x.Count() > 1).ToList();
                throw new Exception($"Duplicate identifier values detected in settings: [{string.Join(", ", duplicateIds.Select(x => $"{x.Key} ({x.Count()})"))}]");
            }

            return Serialize(result);
        }

        private string GetDefaultValue(ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration settingsFieldConfiguration)
        {
            switch (settingsFieldConfiguration.ControlType().AsEnum())
            {
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.Checkbox:
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.Switch:
                    return settingsFieldConfiguration.DefaultValue() ?? "false";
                default:
                    return settingsFieldConfiguration.DefaultValue();
            }
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[] { IntentNugetPackages.IntentPackager };
        }

        private static string Serialize<T>(T @object)
        {
            using var stringWriter = new Utf8StringWriter();
            var xmlSerializer = new XmlSerializer(typeof(T));

            var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            });

            var xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, @object, xmlSerializerNamespaces);

            stringWriter.Close();
            return stringWriter.ToString();
        }

        private ComponentGroupSelectionMode GetSelectionMode(ComponentGroupModel @group)
        {
            switch (@group.GetComponentGroupSettings()?.SelectionMode().AsEnum())
            {
                case ComponentGroupModelStereotypeExtensions.ComponentGroupSettings.SelectionModeOptionsEnum.AllowMultiple:
                    return ComponentGroupSelectionMode.AllowSelectMultiple;
                //return "allow-multiple";
                case ComponentGroupModelStereotypeExtensions.ComponentGroupSettings.SelectionModeOptionsEnum.AllowSingleOnly:
                    return ComponentGroupSelectionMode.AllowSingleOnly;
                //return "allow-single-only";
                default:
                    return ComponentGroupSelectionMode.AllowSelectMultiple;
            }

        }

        private ModuleSettingControlType GetControlType(ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum settingsField)
        {
            switch (settingsField)
            {
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.TextBox:
                    return ModuleSettingControlType.TextBox;
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.Number:
                    return ModuleSettingControlType.Number;
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.Checkbox:
                    return ModuleSettingControlType.Checkbox;
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.Switch:
                    return ModuleSettingControlType.Switch;
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.TextArea:
                    return ModuleSettingControlType.TextArea;
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.Select:
                    return ModuleSettingControlType.Select;
                case ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration.ControlTypeOptionsEnum.MultiSelect:
                    return ModuleSettingControlType.MultiSelect;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settingsField), settingsField, null);
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}