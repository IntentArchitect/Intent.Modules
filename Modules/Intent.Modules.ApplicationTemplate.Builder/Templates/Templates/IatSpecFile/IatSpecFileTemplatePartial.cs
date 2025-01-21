using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.Exceptions;
using Intent.IArchitect.Common.NuGet.Versioning;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Api;
using Intent.Modules.ApplicationTemplate.Builder.Model;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.ApplicationTemplate.Builder.Api.ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration;
using static Intent.Modules.ApplicationTemplate.Builder.Api.ComponentGroupModelStereotypeExtensions.ComponentGroupSettings;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Templates.Templates.IatSpecFile
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
                fileName: "metadata",
                fileExtension: "iatspec",
                relativeLocation: Model.Name
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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
                    Type = Icon.Type,
                    Source = Icon.Source
                },
                Defaults = new ApplicationTemplateDefaults
                {
                    Name = Model.Defaults.Name,
                    RelativeOutputLocation = Model.Defaults.RelativeOutputLocation,
                    PlaceInSameDirectory = Model.Defaults.PlaceSolutionAndApplicationInTheSameDirectory,
                    SeparateIntentFiles = Model.Defaults.StoreIntentArchitectFilesSeparateToCodebase,
                    SetGitIgnoreEntries = Model.Defaults.SetGitIgnoreEntries,
                    CreateFolderForSolution = Model.Defaults.CreateFolderForSolution,
                },
                ComponentGroups = Model.Groups.Select(g => new ApplicationTemplateComponentGroup
                {
                    Name = g.Name,
                    SelectionMode = GetSelectionMode(g),
                    Components = g.Components.Select(c => new ApplicationTemplateComponent()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Included = c.IncludeByDefault,
                        Required = c.IsRequired,
                        Description = c.Description,
                        Icon = new IconModelPersistable()
                        {
                            Type = c.Icon.Type,
                            Source = c.Icon.Source
                        },
                        Modules = c.Modules.Select(m => new ApplicationTemplateComponentModule
                        {
                            Id = m.Name,
                            Version = m.Version,
                            EnableFactoryExtensions = m.EnableFactoryExtensions,
                            InstallApplicationSettings = m.InstallApplicationSettings,
                            InstallDesignerMetadata = m.InstallDesignerMetadata,
                            InstallDesigners = m.InstallDesigners,
                            InstallTemplateOutputs = m.InstallTemplateOutputs,
                            IsIncludedByDefault = m.IsIncludedByDefault,
                            IsRequired = m.IsRequired,
                        }).ToList(),
                        Dependencies = c.GetComponentSettings().Dependencies().Select(d => new ApplicationTemplateComponentDependency()
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
                }).ToList(),
                MinimumDependencyVersions = Model.MinimumDependencyVersions?.Modules
                    .Select(m =>
                    {
                        if (!SemanticVersion.TryParse(m.Value, out _))
                        {
                            throw new ElementException(m.InternalElement, "Value must be a valid semantic version 2 string");
                        }

                        return new MinimumDependencyVersion
                        {
                            Id = m.Name,
                            Version = m.Value
                        };
                    })
                    .ToList()
            };

            if (result.AdditionalSettings.SelectMany(x => x.Settings).GroupBy(x => x.Id.ToLower()).Any(x => x.Count() > 1))
            {
                var duplicateIds = result.AdditionalSettings.SelectMany(x => x.Settings).GroupBy(x => x.Id.ToLower()).Where(x => x.Count() > 1).ToList();
                throw new Exception($"Duplicate identifier values detected in settings: [{string.Join(", ", duplicateIds.Select(x => $"{x.Key} ({x.Count()})"))}]");
            }

            return Serialize(result);
        }

        private static string GetDefaultValue(ApplicationTemplateSettingsFieldConfigurationModelStereotypeExtensions.SettingsFieldConfiguration settingsFieldConfiguration)
        {
            return settingsFieldConfiguration.ControlType().AsEnum() switch
            {
                ControlTypeOptionsEnum.Checkbox or ControlTypeOptionsEnum.Switch => settingsFieldConfiguration.DefaultValue() ?? "false",
                _ => settingsFieldConfiguration.DefaultValue()
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return [IntentNugetPackages.IntentPackager];
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

        private static ComponentGroupSelectionMode GetSelectionMode(ComponentGroupModel group)
        {
            return group.GetComponentGroupSettings()?.SelectionMode().AsEnum() switch
            {
                SelectionModeOptionsEnum.AllowMultiple => ComponentGroupSelectionMode.AllowSelectMultiple,
                SelectionModeOptionsEnum.AllowSingleOnly => ComponentGroupSelectionMode.AllowSingleOnly,
                _ => ComponentGroupSelectionMode.AllowSelectMultiple
            };
        }

        private static ModuleSettingControlType GetControlType(ControlTypeOptionsEnum settingsField)
        {
            return settingsField switch
            {
                ControlTypeOptionsEnum.Hidden => ModuleSettingControlType.Hidden,
                ControlTypeOptionsEnum.TextBox => ModuleSettingControlType.TextBox,
                ControlTypeOptionsEnum.Number => ModuleSettingControlType.Number,
                ControlTypeOptionsEnum.Checkbox => ModuleSettingControlType.Checkbox,
                ControlTypeOptionsEnum.Switch => ModuleSettingControlType.Switch,
                ControlTypeOptionsEnum.TextArea => ModuleSettingControlType.TextArea,
                ControlTypeOptionsEnum.Select => ModuleSettingControlType.Select,
                ControlTypeOptionsEnum.MultiSelect => ModuleSettingControlType.MultiSelect,
                _ => throw new ArgumentOutOfRangeException(nameof(settingsField), settingsField, null)
            };
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }

}