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
using Intent.Modules.ModuleBuilder.Api.Modeler;
using Intent.Templates;
using IconModel = Intent.IArchitect.Agent.Persistence.Model.Common.IconModel;
using IconType = Intent.IArchitect.Common.Types.IconType;
using PackageSettings = Intent.IArchitect.Agent.Persistence.Model.Common.PackageSettings;
using TypeOrder = Intent.IArchitect.Agent.Persistence.Model.Common.TypeOrder;

namespace Intent.Modules.ModuleBuilder.Templates.ModelerConfig
{
    public class ModelerConfig : IntentProjectItemTemplateBase<Modeler>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ModelerConfig";
        private static readonly IconModel _defaultIconModel = new IconModel { Type = IconType.FontAwesome, Source = "file-o" };

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

        private PackageSettings GetPackageSettings(Api.Modeler.PackageSettings settings)
        {
            return new PackageSettings
            {
                CreationOptions = settings.CreationOptions.Select(GetElementCreationOptions).ToList(),
                TypeOrder = settings.TypeOrder.Select(x => new TypeOrder { Type = x.Type, Order = x.Order?.ToString() }).ToList()
            };
            //var packageSettingsElement = GetSingleChildElement(Model, Constants.ElementSpecializationTypes.PackageSettings, allowNull: true);
            //var creationOptionsElement = GetSingleChildElement(packageSettingsElement, Constants.ElementSpecializationTypes.CreationOptions, allowNull: true);

            //var creationOptions = GetElementCreationOptions(creationOptionsElement);

            //return new PackageSettings
            //{
            //    CreationOptions = creationOptions,
            //};
        }

        private ElementCreationOption GetElementCreationOptions(CreationOption option)
        {
            return new ElementCreationOption
            {
                SpecializationType = option.SpecializationType,
                Text = option.Text,
                Shortcut = option.Shortcut,
                DefaultName = option.DefaultName,
                Icon = option.Icon != null ? new IconModel { Type = option.Icon.Type, Source = option.Icon.Source } : _defaultIconModel
            };
        }

        //private List<ElementCreationOption> GetElementCreationOptions(IElement creationOptionsElement)
        //{
        //    return creationOptionsElement?.Attributes
        //        .Where(x => x.SpecializationType == Constants.AttributeSpecializationTypes.CreationOption)
        //        .Select(x =>
        //        {
        //            var elementSettings = x.Type.Element;
        //            var creationOptions = GetCreationOptionsFields(elementSettings, x);

        //            return new ElementCreationOption
        //            {
        //                SpecializationType = elementSettings.Name,
        //                Text = creationOptions.text,
        //                Shortcut = creationOptions.shortcut,
        //                DefaultName = creationOptions.defaultName,
        //                Icon = creationOptions.icon ?? _defaultIconModel
        //            };
        //        }).ToList();
        //}

        //private (string text, string shortcut, string defaultName, IconModel icon, int? typeOrder) GetCreationOptionsFields(IElement elementSettingsElement, IHasStereotypes referencingObject)
        //{
        //    // TODO JL: This is not respecting overrides from the referencingObject, only the defaults for now

        //    var stereotype = GetSingleStereotype(elementSettingsElement, Constants.Stereotypes.DefaultCreationOptions.Name);
        //    var textProperty = GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.Text);
        //    var defaultNameProperty = GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.DefaultName);
        //    var shortcutProperty = GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.Shortcut);
        //    int.TryParse(GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.Shortcut, true)?.Value, out var typeOrderProperty);
        //    var iconModel = GetIcon(elementSettingsElement, overrideStereotypes: referencingObject);

        //    return (
        //        text: ValueOrFallback(textProperty, $"New {elementSettingsElement.Name}"),
        //        shortcut: ValueOrFallback(shortcutProperty, null),
        //        defaultName: ValueOrFallback(defaultNameProperty, $"New {elementSettingsElement.Name}"),
        //        icon: iconModel,
        //        typeOrder: typeOrderProperty);
        //}

        private IconModel GetIcon(Api.Modeler.IconModel icon)
        {
            return icon != null ? new IconModel { Type = icon.Type, Source = icon.Source } : null;
        }

        private IconModel GetIcon(IElement elementSettingsElement, bool expanded = false, IHasStereotypes overrideStereotypes = null)
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

            return new IconModel
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

        private List<AssociationSettings> GetAssociationSettings(IList<AssociationSetting> associationSettings)
        {
            return associationSettings.Select(x => new AssociationSettings
            {
                SpecializationType = x.SpecializationType,
                Icon = GetIcon(x.Icon),
                SourceEnd = new AssociationEndSettings
                {
                    TargetTypes = x.SourceEnd.TargetTypes,
                    IsCollectionDefault = x.SourceEnd.IsCollectionDefault,
                    IsCollectionEnabled = x.SourceEnd.IsCollectionEnabled,
                    IsNavigableDefault = x.SourceEnd.IsNavigableEnabled,
                    IsNavigableEnabled = x.SourceEnd.IsNavigableEnabled,
                    IsNullableDefault = x.SourceEnd.IsNullableDefault,
                    IsNullableEnabled = x.SourceEnd.IsNullableEnabled
                },
                TargetEnd = new AssociationEndSettings
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
            //var items = GetAllChildElementsOfTypes(Model, Constants.ElementSpecializationTypes.AssociationSettings)
            //    .OrderBy(x => x.Name)
            //    .Distinct()
            //    .ToArray();

            //return items
            //    .Select(associationSettingsElement =>
            //    {
            //        var sourceEnd =
            //            GetSingleChildElement(associationSettingsElement, Constants.ElementSpecializationTypes.SourceEnd);
            //        var destinationEnd =
            //            GetSingleChildElement(associationSettingsElement, Constants.ElementSpecializationTypes.DestinationEnd);

            //        var sourceEndSettings =
            //            GetSingleChildElement(sourceEnd, Constants.ElementSpecializationTypes.AssociationEndSettings);
            //        var destinationEndSettings =
            //            GetSingleChildElement(destinationEnd, Constants.ElementSpecializationTypes.AssociationEndSettings);

            //        var sourceEndAdditionalProperties =
            //            GetSingleStereotype(sourceEndSettings, Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Name);
            //        var destinationEndAdditionalProperties =
            //            GetSingleStereotype(destinationEndSettings, Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Name);

            //        return new AssociationSettings
            //        {
            //            SpecializationType = associationSettingsElement.Name,
            //            Icon = GetIcon(associationSettingsElement) ?? _defaultIconModel, // Confirm the default is needed
            //            SourceEnd = new AssociationEndSettings
            //            {
            //                TargetTypes = sourceEndSettings.Attributes
            //                    .Select(x => x.Type.Element.Name)
            //                    .ToArray(),
            //                IsNavigableEnabled = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableEnabled),
            //                IsNullableEnabled = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableEnabled),
            //                IsCollectionEnabled = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionEnabled),
            //                IsNavigableDefault = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableDefault),
            //                IsNullableDefault = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableDefault),
            //                IsCollectionDefault = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionDefault),
            //            },
            //            TargetEnd = new AssociationEndSettings
            //            {
            //                TargetTypes = destinationEndSettings.Attributes
            //                .Select(x => x.Type.Element.Name)
            //                .ToArray(),
            //                IsNavigableEnabled = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableEnabled),
            //                IsNullableEnabled = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableEnabled),
            //                IsCollectionEnabled = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionEnabled),
            //                IsNavigableDefault = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableDefault),
            //                IsNullableDefault = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableDefault),
            //                IsCollectionDefault = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
            //                    Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionDefault),
            //            }
            //        };
            //    })
            //    .ToList();
        }

        private List<ElementSettings> GetElementSettings(IList<Api.Modeler.ElementSetting> elementSettings)
        {
            return elementSettings.Select(x =>
                new ElementSettings
                {
                    SpecializationType = x.SpecializationType,
                    Icon = GetIcon(x.Icon),
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
                    OperationSettings = x.OperationSettings?.Any() == true
                        ? x.OperationSettings.Select(GetOperationSettings).ToArray()
                        : null,
                    MappingSettings = null, // TODO JL
                    CreationOptions = x.CreationOptions?.Any() == true
                        ? x.CreationOptions.Select(GetElementCreationOptions).ToList()
                        : null,
                    TypeOrder = x.TypeOrder?.Any() == true
                        ? x.TypeOrder.Select((t, index) => new TypeOrder { Type = t.Type, Order = t.Order?.ToString() }).ToList()
                        : null
                })
                .ToList();
            //var items = GetAllChildElementsOfTypes(Model, Constants.ElementSpecializationTypes.ElementSettings)
            //    .OrderBy(x => x.Name)
            //    .ToArray();

            //var duplicates = items
            //    .GroupBy(x => x.Name)
            //    .Where(x => x.Count() > 1)
            //    .Select(x => x.Key)
            //    .DefaultIfEmpty()
            //    .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");
            //if (!string.IsNullOrWhiteSpace(duplicates))
            //{
            //    throw new Exception($"More than one '{Constants.ElementSpecializationTypes.ElementSettings}' " +
            //                        $"exist for the following:{Environment.NewLine}{duplicates}");
            //}

            //return items
            //    .Select(elementSettingsElement =>
            //    {
            //        var creationOptions =
            //            GetElementCreationOptions(GetSingleChildElement(elementSettingsElement, Constants.ElementSpecializationTypes.CreationOptions, true))?
            //            .ToList();

            //        var additionalProperties =
            //            GetSingleStereotype(elementSettingsElement, Constants.Stereotypes.ElementAdditionalProperties.Name);

            //        var attributeSettings =
            //            GetSingleChildElement(elementSettingsElement, Constants.ElementSpecializationTypes.AttributeSettings, true)?
            //            .ChildElements.Where(x => x.SpecializationType == Constants.ElementSpecializationTypes.AttributeSetting)
            //            .Select(GetAttributeSetting)
            //            .ToArray();

            //        var typeOrder = (creationOptions ?? Enumerable.Empty<ElementCreationOption>())
            //            .Select(x => x.SpecializationType)
            //            .Union((attributeSettings ?? Enumerable.Empty<AttributeSettings>()).Select(x =>
            //                x.SpecializationType))
            //            .Distinct()
            //            .Select(x => new TypeOrder() { Type = x })
            //            .ToList();

            //        return new ElementSettings
            //        {
            //            SpecializationType = elementSettingsElement.Name,
            //            Icon = GetIcon(elementSettingsElement),
            //            ExpandedIcon = GetIcon(elementSettingsElement, expanded: true),
            //            AllowRename = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowRename),
            //            AllowAbstract = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowAbstract),
            //            AllowGenericTypes = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowGenericTypes),
            //            AllowMapping = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowMapping),
            //            AllowSorting = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowSorting),
            //            AllowFindInView = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowFindInView),
            //            DiagramSettings = null, // TODO JL
            //            LiteralSettings = GetLiteralSettings(elementSettingsElement), // TODO JL
            //            AttributeSettings = attributeSettings?.Any() == true
            //                ? attributeSettings
            //                : null,
            //            OperationSettings = null, // TODO JL
            //            MappingSettings = null, // TODO JL
            //            CreationOptions = creationOptions?.Any() == true
            //                ? creationOptions
            //                : null,
            //            TypeOrder = typeOrder.Any()
            //                ? typeOrder
            //                : null
            //        };
            //    })
            //    .ToList();
        }

        private ClassLiteralSettings GetLiteralSettings(LiteralSetting literal)
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
            //var items = GetAllChildElementsOfTypes(elementSettingsElement, Constants.ElementSpecializationTypes.LiteralSettings)
            //    .OrderBy(x => x.Name)
            //    .ToArray();

            //if (!items.Any())
            //{
            //    return null;
            //}

            //return items
            //    .Select(element =>
            //    {
            //        var additionalProperties = GetSingleStereotype(element, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Name);
            //        var text = GetSingleProperty(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.Text);
            //        var shortcut = GetSingleProperty(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.Shortcut);
            //        var defaultName = GetSingleProperty(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.DefaultName);

            //        return new ClassLiteralSettings
            //        {
            //            SpecializationType = element.Name,
            //            Icon = GetIcon(element) ?? _defaultIconModel, // TODO JL: Check if the default actually needed
            //            Text = ValueOrFallback(text, $"New {element.Name}"),
            //            Shortcut = ValueOrFallback(shortcut, null),
            //            DefaultName = ValueOrFallback(defaultName, $"New {element.Name}"),
            //            AllowRename = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.AllowRename),
            //            AllowDuplicateNames = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.AllowDuplicateNames),
            //            AllowFindInView = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.AllowFindInView)
            //        };
            //    })
            //    .ToArray();
        }

        

        private AttributeSettings GetAttributeSettings(AttributeSetting setting)
        {
            return new AttributeSettings()
            {
                SpecializationType = setting.SpecializationType,
                Icon = GetIcon(setting.Icon) ?? _defaultIconModel,
                Text = setting.Text,
                Shortcut = setting.Shortcut,
                DisplayFunction = setting.DisplayFunction,
                DefaultName = setting.DefaultName,
                AllowRename = setting.AllowRename,
                AllowDuplicateNames = setting.AllowDuplicateNames,
                AllowFindInView = setting.AllowFindInView,
                DefaultTypeId = setting.DefaultTypeId,
                TargetTypes = setting.TargetTypes
            };
            //var additionalProperties = GetSingleStereotype(element, Constants.Stereotypes.AttributeAdditionalProperties.Name);
            //var text = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.Text);
            //var shortcut = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.Shortcut);
            //var displayFunction = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.DisplayFunction);
            //var defaultName = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.DefaultName);
            //var allowRename = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.AllowRename);
            //var allowDuplicateNames = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.AllowDuplicateNames);
            //var allowFindInView = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.AllowFindInView);
            //var defaultTypeId = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.DefaultTypeId);

            //return new AttributeSettings
            //{
            //    SpecializationType = element.Name,
            //    Icon = GetIcon(element) ?? _defaultIconModel,
            //    Text = ValueOrFallback(text, $"New {element.Name}"),
            //    Shortcut = ValueOrFallback(shortcut, null),
            //    DisplayFunction = ValueOrFallback(displayFunction, null),
            //    DefaultName = ValueOrFallback(defaultName, $"New {element.Name}"),
            //    AllowRename = allowRename,
            //    AllowDuplicateNames = allowDuplicateNames,
            //    AllowFindInView = allowFindInView,
            //    DefaultTypeId = ValueOrFallback(defaultTypeId, null),
            //    TargetTypes = element.Attributes
            //        .Select(x => x.Type.Element.Name)
            //        .ToArray()
            //};
        }

        private OperationSettings GetOperationSettings(OperationSetting setting)
        {
            return new OperationSettings()
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

        private static IEnumerable<IElement> GetAllChildElementsOfTypes(IElement element, params string[] specializationTypes)
        {
            foreach (var childElement in element.ChildElements)
            {
                if (specializationTypes.Contains(childElement.SpecializationType))
                {
                    yield return childElement;
                }

                foreach (var folderChildElement in GetAllChildElementsOfTypes(childElement, specializationTypes))
                {
                    yield return folderChildElement;
                }
            }
        }

        private static string ValueOrFallback(IStereotypeProperty property, string fallback)
        {
            if (property.Value == "_")
            {
                return string.Empty;
            }

            return !string.IsNullOrWhiteSpace(property.Value)
                ? property.Value
                : fallback;
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

        private static bool GetSinglePropertyAsBool(IStereotype stereotype, string key)
        {
            return true.ToString().Equals(GetSingleProperty(stereotype, key).Value, StringComparison.OrdinalIgnoreCase);
        }

        private static IElement GetSingleChildElement(IElement parentElement, string specializationType, bool allowNull = false)
        {
            try
            {
                return allowNull
                    ? parentElement?.ChildElements.SingleOrDefault(x => x.SpecializationType == specializationType)
                    : parentElement.ChildElements.Single(x => x.SpecializationType == specializationType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Expected single ChildElement of type [{specializationType}]", e);
            }
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}.modeler",
                fileExtension: "config",
                defaultLocationInProject: "modeler");
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