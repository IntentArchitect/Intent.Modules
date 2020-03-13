using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModelerBuilder;
using Intent.Modules.ModelerBuilder.External;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;
using AttributeSettings = Intent.Modules.ModuleBuilder.Api.AttributeSettings;
using IconType = Intent.IArchitect.Common.Types.IconType;
using TypeOrder = Intent.IArchitect.Agent.Persistence.Model.Common.TypeOrder;

namespace Intent.Modules.ModuleBuilder.Templates.ModelerConfig
{
    public class ModelerConfig : IntentProjectItemTemplateBase<Modeler>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ModelerConfig";
        private static readonly IconModelPersistable _defaultIconModel = new IconModelPersistable { Type = IconType.FontAwesome, Source = "file-o" };

        public ModelerConfig(IProject project, Modeler model) : base(TemplateId, project, model)
        {
        }

        public override string TransformText()
        {
            var path = FileMetadata.GetFullLocationPathWithFileName();
            var applicationModelerModeler = File.Exists(path)
                ? LoadAndDeserialize<ApplicationModelerModel>(path)
                : new ApplicationModelerModel { Settings = new ModelerSettings() };

            var modelerSettings = applicationModelerModeler.Settings;

            //modelerSettings.DiagramSettings // TODO JL
            modelerSettings.PackageSettings = GetPackageSettings(Model.PackageSettings);
            modelerSettings.ElementSettings = GetElementSettings(Model.ElementSettings);
            modelerSettings.AssociationSettings = GetAssociationSettings(Model.AssociationSettings);
            modelerSettings.StereotypeSettings = GetStereotypeSettings(Model);

            return Serialize(applicationModelerModeler);
        }

        private PackageSettingsPersistable GetPackageSettings(PackageSettings settings)
        {
            return new PackageSettingsPersistable
            {
                CreationOptions = settings.CreationOptions.Select(GetElementCreationOptions).ToList(),
                TypeOrder = settings.TypeOrder.Select(x => new TypeOrder { Type = x.Type, Order = x.Order?.ToString() }).ToList()
            };
        }

        private ElementCreationOption GetElementCreationOptions(ICreationOption option)
        {
            return new ElementCreationOption
            {
                SpecializationType = option.SpecializationType,
                Text = option.Text,
                Shortcut = option.Shortcut,
                DefaultName = option.DefaultName,
                Icon = GetIcon(option.Icon) ?? _defaultIconModel
            };
        }

        private IconModelPersistable GetIcon(IconModel icon)
        {
            return icon != null ? new IconModelPersistable { Type = icon.Type, Source = icon.Source } : null;
        }

        private IconModelPersistable GetIcon(IElement elementSettingsElement, bool expanded = false, IHasStereotypes overrideStereotypes = null)
        {
            // TODO JL: Referenced icons not implemented

            var iconStereotypeName = expanded
                ? Constants.Stereotypes.IconFullExpanded.Name
                : Constants.Stereotypes.IconFull.Name;

            var icon =
                GetSingleStereotype(overrideStereotypes, iconStereotypeName, true) ??
                GetSingleStereotype(elementSettingsElement, iconStereotypeName, true);

            if (icon == null)
            {
                return null;
            }

            var type = GetSingleProperty(icon, Constants.Stereotypes.IconFull.Property.Type);
            var source = GetSingleProperty(icon, Constants.Stereotypes.IconFull.Property.Source);

            if (string.IsNullOrWhiteSpace(type.Value))
            {
                return null;
            }

            return new IconModelPersistable
            {
                Type = (IconType)Enum.Parse(typeof(IconType), type.Value),
                Source = source.Value
            };
        }

        private StereotypeSettings GetStereotypeSettings(Modeler model)
        {
            var targetTypes = model.ElementSettings.Select(x => x.SpecializationType)
                .Concat(model.ElementSettings.SelectMany(x => x.AttributeSettings).Select(x => x.SpecializationType))
                .Concat(model.AssociationSettings.Select(x => x.SpecializationType))
                .ToList();

            return new StereotypeSettings
            {
                TargetTypeOptions = targetTypes.Select(x => new StereotypeTargetTypeOption()
                {
                    SpecializationType = x,
                    DisplayText = x
                }).ToList()
            };
        }

        private List<AssociationSettingsPersistable> GetAssociationSettings(IList<AssociationSetting> associationSettings)
        {
            return associationSettings.Select(x => new AssociationSettingsPersistable
            {
                SpecializationType = x.SpecializationType,
                Icon = GetIcon(x.Icon),
                SourceEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = x.SourceEnd.TargetTypes,
                    IsCollectionDefault = x.SourceEnd.IsCollectionDefault,
                    IsCollectionEnabled = x.SourceEnd.IsCollectionEnabled,
                    IsNavigableDefault = x.SourceEnd.IsNavigableEnabled,
                    IsNavigableEnabled = x.SourceEnd.IsNavigableEnabled,
                    IsNullableDefault = x.SourceEnd.IsNullableDefault,
                    IsNullableEnabled = x.SourceEnd.IsNullableEnabled
                },
                TargetEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = x.TargetEnd.TargetTypes,
                    IsCollectionDefault = x.TargetEnd.IsCollectionDefault,
                    IsCollectionEnabled = x.TargetEnd.IsCollectionEnabled,
                    IsNavigableDefault = x.TargetEnd.IsNavigableEnabled,
                    IsNavigableEnabled = x.TargetEnd.IsNavigableEnabled,
                    IsNullableDefault = x.TargetEnd.IsNullableDefault,
                    IsNullableEnabled = x.TargetEnd.IsNullableEnabled
                },
            }).ToList();
        }

        private List<ElementSettingsPersistable> GetElementSettings(IList<ElementSettings> elementSettings)
        {
            return elementSettings.Select(x =>
                new ElementSettingsPersistable
                {
                    SpecializationType = x.SpecializationType,
                    Icon = GetIcon(x.Icon) ?? _defaultIconModel,
                    ExpandedIcon = GetIcon(x.ExpandedIcon),
                    AllowRename = x.AllowRename,
                    AllowAbstract = x.AllowAbstract,
                    AllowGenericTypes = x.AllowGenericTypes,
                    AllowMapping = x.AllowMapping,
                    AllowSorting = x.AllowSorting,
                    AllowFindInView = x.AllowFindInView,
                    DiagramSettings = null, // TODO JL
                    LiteralSettings = x.LiteralSettings?.Any() == true
                        ? x.LiteralSettings.Select(GetLiteralSettings).ToArray()
                        : null,
                    AttributeSettings = x.AttributeSettings?.Any() == true
                        ? x.AttributeSettings.Select(GetAttributeSettings).ToArray()
                        : null,
                    OperationSettingsPersistable = x.OperationSettings?.Any() == true
                        ? x.OperationSettings.Select(GetOperationSettings).ToArray()
                        : null,
                    ChildElementSettings = GetElementSettings(x.ChildElementSettings).ToArray(),
                    MappingSettings = null, // TODO JL
                    CreationOptions = x.CreationOptions != null
                        ? x.CreationOptions.Options.Select(GetElementCreationOptions).ToList()
                        : null,
                    TypeOrder = x.TypeOrder?.Any() == true
                        ? x.TypeOrder.Select((t, index) => new TypeOrder { Type = t.Type, Order = t.Order?.ToString() }).ToList()
                        : null
                })
                .ToList();
        }

        private ClassLiteralSettings GetLiteralSettings(LiteralSettings literal)
        {
            return new ClassLiteralSettings
            {
                SpecializationType = literal.SpecializationType,
                Icon = GetIcon(literal.Icon) ?? _defaultIconModel, // TODO JL: Check if the default actually needed
                Text = literal.Text,
                Shortcut = literal.Shortcut,
                DefaultName = literal.DefaultName,
                AllowRename = literal.AllowRename,
                AllowDuplicateNames = literal.AllowDuplicateNames,
                AllowFindInView = literal.AllowFindInView
            };
        }

        
        private IArchitect.Agent.Persistence.Model.Common.AttributeSettings GetAttributeSettings(AttributeSettings settings)
        {
            return new IArchitect.Agent.Persistence.Model.Common.AttributeSettings()
            {
                SpecializationType = settings.SpecializationType,
                Icon = GetIcon(settings.Icon) ?? _defaultIconModel,
                Text = settings.Text,
                Shortcut = settings.Shortcut,
                DisplayFunction = settings.DisplayFunction,
                DefaultName = settings.DefaultName,
                AllowRename = settings.AllowRename,
                AllowDuplicateNames = settings.AllowDuplicateNames,
                AllowFindInView = settings.AllowFindInView,
                DefaultTypeId = settings.DefaultTypeId,
                TargetTypes = settings.TargetTypes
            };
        }

        private OperationSettingsPersistable GetOperationSettings(OperationSetting setting)
        {
            return new OperationSettingsPersistable()
            {
                SpecializationType = setting.SpecializationType,
                Icon = GetIcon(setting.Icon) ?? _defaultIconModel,
                Text = setting.Text,
                Shortcut = setting.Shortcut,
                DefaultName = setting.DefaultName,
                AllowRename = setting.AllowRename,
                AllowDuplicateNames = setting.AllowDuplicateNames,
                AllowFindInView = setting.AllowFindInView,
                DefaultTypeId = setting.DefaultTypeId,
                TargetTypes = setting.TargetTypes
            };
        }

        private static IStereotype GetSingleStereotype(IHasStereotypes hasStereotypes, string name, bool allowNull = false)
        {
            try
            {
                return allowNull
                    ? hasStereotypes?.Stereotypes.SingleOrDefault(x => x.Name == name)
                    : hasStereotypes.Stereotypes.Single(x => x.Name == name);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Expected single stereotype with name [{name}]", e);
            }
        }

        private static IStereotypeProperty GetSingleProperty(IStereotype stereotype, string key, bool allowNull = false)
        {
            try
            {
                return allowNull
                    ? stereotype?.Properties.SingleOrDefault(x => x.Key == key)
                    : stereotype.Properties.Single(x => x.Key == key);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Expected single stereotype property with key [{key}]", e);
            }
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}.modeler{(Model.IsExtension ? ".extension" : "")}",
                fileExtension: "config",
                defaultLocationInProject: "modelers");
        }

        private static T LoadAndDeserialize<T>(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }

        private static string Serialize<T>(T @object)
        {
            using (var stringWriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                var serializerNamespaces = new XmlSerializerNamespaces();
                serializerNamespaces.Add("", "");

                serializer.Serialize(stringWriter, @object, serializerNamespaces);
                stringWriter.Close();

                return stringWriter.ToString();
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

    }
}