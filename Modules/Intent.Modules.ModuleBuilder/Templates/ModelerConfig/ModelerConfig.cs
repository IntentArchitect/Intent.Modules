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
    public class ModelerConfig : IntentProjectItemTemplateBase<IModeler>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ModelerConfig";

        private static readonly IconModelPersistable _defaultIconModel = new IconModelPersistable { Type = IconType.FontAwesome, Source = "file-o" };

        public ModelerConfig(IProject project, IModeler model) : base(TemplateId, project, model)
        {
        }

        public override string TransformText()
        {
            var path = FileMetadata.GetFullLocationPathWithFileName();
            var applicationModelerModeler = File.Exists(path)
                ? LoadAndDeserialize<ApplicationModelerModel>(path)
                : new ApplicationModelerModel { Settings = new ModelerSettingsPersistable() };

            var modelerSettings = applicationModelerModeler.Settings;

            //modelerSettings.DiagramSettings // TODO JL
            modelerSettings.PackageSettings = GetPackageSettings(Model.PackageSettings);
            modelerSettings.ElementSettings = GetElementSettings(Model.ElementTypes);
            modelerSettings.AssociationSettings = GetAssociationSettings(Model.AssociationTypes);
            modelerSettings.StereotypeSettings = GetStereotypeSettings(Model);

            return Serialize(applicationModelerModeler);
        }

        private PackageSettingsPersistable GetPackageSettings(IPackageSettings settings)
        {
            return new PackageSettingsPersistable
            {
                CreationOptions = settings?.MenuOptions.CreationOptions.Select(GetElementCreationOptions).ToList(),
                TypeOrder = settings?.MenuOptions.TypeOrder.Select(x => new TypeOrder { Type = x.Type, Order = x.Order?.ToString() }).ToList()
            };
        }

        private ElementCreationOption GetElementCreationOptions(ICreationOption option)
        {
            return new ElementCreationOption
            {
                SpecializationType = option.TargetSpecializationType,
                Text = option.Text,
                Shortcut = option.Shortcut,
                DefaultName = option.DefaultName,
                Icon = GetIcon(option.Icon) ?? _defaultIconModel,
                AllowMultiple = option.AllowMultiple
            };
        }

        private IconModelPersistable GetIcon(ElementSettingsExtensions.IconFullExpanded icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
        }

        private IconModelPersistable GetIcon(ElementSettingsExtensions.IconFull icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
        }

        private IconModelPersistable GetIcon(LiteralSettingsExtensions.IconFull icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
        }

        private IconModelPersistable GetIcon(AssociationSettingsExtensions.IconFull icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
        }

        private IconModelPersistable GetIcon(IconModel icon)
        {
            return icon != null ? new IconModelPersistable { Type = icon.Type, Source = icon.Source } : null;
        } 

        private StereotypeSettingsPersistable GetStereotypeSettings(IModeler model)
        {
            var targetTypes = model.ElementTypes.Select(x => x.Name)
                .Concat(model.ElementTypes.SelectMany(x => x.ChildElementSettings).Select(x => x.Name))
                .Concat(model.ElementTypes.SelectMany(x => x.LiteralSettings).Select(x => x.Name))
                .Concat(model.ElementTypes.SelectMany(x => x.AttributeSettings).Select(x => x.Name))
                .Concat(model.ElementTypes.SelectMany(x => x.OperationSettings).Select(x => x.Name))
                .Concat(model.AssociationTypes.Select(x => x.Name))
                .Concat(new [] { "Package" })
                .OrderBy(x => x)
                .ToList();

            return new StereotypeSettingsPersistable
            {
                TargetTypeOptions = targetTypes.Select(x => new StereotypeTargetTypeOption()
                {
                    SpecializationType = x,
                    DisplayText = x
                }).ToList()
            };
        }

        private List<AssociationSettingsPersistable> GetAssociationSettings(IList<IAssociationSettings> associationSettings)
        {
            return associationSettings.OrderBy(x => x.Name).Select(x => new AssociationSettingsPersistable
            {
                SpecializationType = x.Name,
                Icon = GetIcon(x.GetIconFull()),
                SourceEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = x.SourceEnd.GetAdditionalProperties().TargetTypes().Select(t => t.Name).ToArray(),
                    IsCollectionDefault = x.SourceEnd.GetAdditionalProperties().IsCollectionDefault(),
                    IsCollectionEnabled = x.SourceEnd.GetAdditionalProperties().IsCollectionEnabled(),
                    IsNavigableDefault = x.SourceEnd.GetAdditionalProperties().IsNavigableEnabled(),
                    IsNavigableEnabled = x.SourceEnd.GetAdditionalProperties().IsNavigableEnabled(),
                    IsNullableDefault = x.SourceEnd.GetAdditionalProperties().IsNullableDefault(),
                    IsNullableEnabled = x.SourceEnd.GetAdditionalProperties().IsNullableEnabled()
                },
                TargetEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = x.DestinationEnd.GetAdditionalProperties().TargetTypes().Select(t => t.Name).ToArray(),
                    IsCollectionDefault = x.DestinationEnd.GetAdditionalProperties().IsCollectionDefault(),
                    IsCollectionEnabled = x.DestinationEnd.GetAdditionalProperties().IsCollectionEnabled(),
                    IsNavigableDefault = x.DestinationEnd.GetAdditionalProperties().IsNavigableEnabled(),
                    IsNavigableEnabled = x.DestinationEnd.GetAdditionalProperties().IsNavigableEnabled(),
                    IsNullableDefault = x.DestinationEnd.GetAdditionalProperties().IsNullableDefault(),
                    IsNullableEnabled = x.DestinationEnd.GetAdditionalProperties().IsNullableEnabled()
                },
            }).ToList();
        }

        private List<ElementSettingsPersistable> GetElementSettings(IList<IElementSettings> elementSettings)
        {
            return elementSettings.OrderBy(x => x.Name).Select(x =>
                new ElementSettingsPersistable
                {
                    SpecializationType = x.Name,
                    Icon = GetIcon(x.GetIconFull()) ?? _defaultIconModel,
                    ExpandedIcon = GetIcon(x.GetIconFullExpanded()),
                    AllowRename = x.GetAdditionalProperties().AllowRename(),
                    AllowAbstract = x.GetAdditionalProperties().AllowAbstract(),
                    AllowGenericTypes = x.GetAdditionalProperties().AllowGenericTypes(),
                    AllowMapping = x.GetAdditionalProperties().AllowMapping(),
                    AllowSorting = x.GetAdditionalProperties().AllowSorting(),
                    AllowFindInView = x.GetAdditionalProperties().AllowFindinView(),
                    AllowTypeReference = x.GetAdditionalProperties().AllowTypeReference(),
                    TargetTypes = x.GetAdditionalProperties().TargetTypes()?.Select(e => e.Name).ToArray(),
                    DefaultTypeId = x.GetAdditionalProperties().DefaultTypeId(),
                    DiagramSettings = null, // TODO JL / GCB
                    LiteralSettings = x.LiteralSettings?.Any() == true
                        ? x.LiteralSettings.Select(GetLiteralSettings).ToArray()
                        : null,
                    AttributeSettings = x.AttributeSettings?.Any() == true
                        ? x.AttributeSettings.Select(GetAttributeSettings).ToArray()
                        : null,
                    OperationSettings = x.OperationSettings?.Any() == true
                        ? x.OperationSettings.Select(GetOperationSettings).ToArray()
                        : null,
                    ChildElementSettings = GetElementSettings(x.ChildElementSettings).ToArray(),
                    MappingSettings = null, // TODO JL
                    CreationOptions = x.MenuOptions?.CreationOptions.Select(GetElementCreationOptions).ToList(),
                    TypeOrder = x.MenuOptions?.TypeOrder.Select((t, index) => new TypeOrder { Type = t.Type, Order = t.Order?.ToString() }).ToList()
                })
                .ToList();
        }

        private ClassLiteralSettings GetLiteralSettings(ILiteralSettings literal)
        {
            return new ClassLiteralSettings
            {
                SpecializationType = literal.Name,
                Icon = GetIcon(literal.GetIconFull()) ?? _defaultIconModel,
                Text = literal.GetAdditionalProperties().Text(),
                Shortcut = literal.GetAdditionalProperties().Shortcut(),
                DefaultName = literal.GetAdditionalProperties().DefaultName(),
                AllowRename = literal.GetAdditionalProperties().AllowRename(),
                AllowDuplicateNames = literal.GetAdditionalProperties().AllowDuplicateNames(),
                AllowFindInView = literal.GetAdditionalProperties().AllowFindinView()
            };
        }


        private AttributeSettingsPersistable GetAttributeSettings(IAttributeSettings settings)
        {
            return new AttributeSettingsPersistable
            {
                SpecializationType = settings.Name,
                Icon = GetIcon(settings.Icon) ?? _defaultIconModel,
                Text = settings.GetAdditionalProperties().Text(),
                Shortcut = settings.GetAdditionalProperties().Shortcut(),
                DisplayFunction = settings.GetAdditionalProperties().DisplayFunction(),
                DefaultName = settings.GetAdditionalProperties().DefaultName(),
                AllowRename = settings.GetAdditionalProperties().AllowRename(),
                AllowDuplicateNames = settings.GetAdditionalProperties().AllowDuplicateNames(),
                AllowFindInView = settings.GetAdditionalProperties().AllowFindinView(),
                DefaultTypeId = settings.GetAdditionalProperties().DefaultTypeId(),
                TargetTypes = settings.GetAdditionalProperties().TargetTypes().Select(x => x.Name).ToArray()
            };
        }

        private OperationSettingsPersistable GetOperationSettings(IOperationSettings settings)
        {
            return new OperationSettingsPersistable()
            {
                SpecializationType = settings.Name,
                Icon = GetIcon(settings.Icon) ?? _defaultIconModel,
                Text = settings.GetAdditionalProperties().Text(),
                Shortcut = settings.GetAdditionalProperties().Shortcut(),
                DefaultName = settings.GetAdditionalProperties().DefaultName(),
                AllowRename = settings.GetAdditionalProperties().AllowRename(),
                AllowDuplicateNames = settings.GetAdditionalProperties().AllowDuplicateNames(),
                AllowFindInView = settings.GetAdditionalProperties().AllowFindinView(),
                DefaultTypeId = settings.GetAdditionalProperties().DefaultTypeId(),
                TargetTypes = settings.GetAdditionalProperties().TargetTypes().Select(x => x.Name).ToArray()
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
                fileName: $"{Model.Name}.modeler{(Model.GetModelerSettings().ModelerType().IsExtension() ? ".extension" : "")}",
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