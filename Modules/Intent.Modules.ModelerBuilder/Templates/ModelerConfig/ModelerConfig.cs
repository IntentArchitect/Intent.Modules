using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Common.Types;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModelerBuilder.External;
using Intent.Templates;

namespace Intent.Modules.ModelerBuilder.Templates.ModelerConfig
{
    public class ModelerConfig : IntentProjectItemTemplateBase<IElement>
    {
        public const string TemplateId = "ModelerBuilder.Templates.ModelerConfig";
        private static readonly IconModel _defaultIconModel = new IconModel { Type = IconType.FontAwesome, Source = "file-o" };

        public ModelerConfig(IProject project, IElement model) : base(TemplateId, project, model)
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
            modelerSettings.PackageSettings = GetPackageSettings();
            modelerSettings.ElementSettings = GetElementSettings();
            modelerSettings.AssociationSettings = GetAssociationSettings();
            modelerSettings.StereotypeSettings = GetStereotypeSettings();

            return Serialize(applicationModelerModeler);
        }

        private PackageSettings GetPackageSettings()
        {
            var packageSettingsElement = GetSingleChildElement(Model, Constants.ElementSpecializationTypes.PackageSettings);
            var creationOptionsElement = GetSingleChildElement(packageSettingsElement, Constants.ElementSpecializationTypes.CreationOptions);

            var creationOptions = GetElementCreationOptions(creationOptionsElement).ToArray();

            return new PackageSettings
            {
                CreationOptions = creationOptions,
                TypeOrder = creationOptions.Select(x => x.SpecializationType).ToArray()
            };
        }

        private IEnumerable<ElementCreationOption> GetElementCreationOptions(IElement creationOptionsElement)
        {
            return creationOptionsElement?.Attributes
                .Where(x => x.SpecializationType == Constants.AttributeSpecializationTypes.CreationOption)
                .Select(x =>
                {
                    var elementSettings = x.Type.Element;
                    var creationOptions = GetCreationOptionsFields(elementSettings, x);

                    return new ElementCreationOption
                    {
                        SpecializationType = elementSettings.Name,
                        Text = creationOptions.text,
                        Shortcut = creationOptions.shortcut,
                        DefaultName = creationOptions.defaultName,
                        Icon = creationOptions.icon ?? _defaultIconModel
                    };
                });
        }

        private (string text, string shortcut, string defaultName, IconModel icon) GetCreationOptionsFields(IElement elementSettingsElement, IHasStereotypes referencingObject)
        {
            // TODO JL: This is not respecting overrides from the referencingObject, only the defaults for now

            var stereotype = GetSingleStereotype(elementSettingsElement, Constants.Stereotypes.DefaultCreationOptions.Name);
            var textProperty = GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.Text);
            var defaultNameProperty = GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.DefaultName);
            var shortcutProperty = GetSingleProperty(stereotype, Constants.Stereotypes.DefaultCreationOptions.Property.Shortcut);
            var iconModel = GetIcon(elementSettingsElement, overrideStereotypes: referencingObject);

            return (
                text: ValueOrFallback(textProperty, $"New {elementSettingsElement.Name}"),
                shortcut: ValueOrFallback(shortcutProperty, null),
                defaultName: ValueOrFallback(defaultNameProperty, $"New {elementSettingsElement.Name}"),
                icon: iconModel);
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

        private StereotypeSettings GetStereotypeSettings()
        {
            var targetTypeOptions = GetAllChildElementsOfTypes(Model,
                    Constants.ElementSpecializationTypes.ElementSettings,
                    Constants.ElementSpecializationTypes.AttributeSetting)
                .Select(x => new StereotypeTargetTypeOption
                {
                    SpecializationType = x.Name,
                    DisplayText = x.Name
                })
                .OrderBy(x => x.SpecializationType)
                .ThenBy(x => x.DisplayText)
                .ToArray();

            return new StereotypeSettings
            {
                TargetTypeOptions = targetTypeOptions
            };
        }

        private AssociationSettings[] GetAssociationSettings()
        {
            var items = GetAllChildElementsOfTypes(Model, Constants.ElementSpecializationTypes.AssociationSettings)
                .OrderBy(x => x.Name)
                .ToArray();

            return items
                .Select(associationSettingsElement =>
                {
                    var sourceEnd =
                        GetSingleChildElement(associationSettingsElement, Constants.ElementSpecializationTypes.SourceEnd);
                    var destinationEnd =
                        GetSingleChildElement(associationSettingsElement, Constants.ElementSpecializationTypes.DestinationEnd);

                    var sourceEndSettings =
                        GetSingleChildElement(sourceEnd, Constants.ElementSpecializationTypes.AssociationEndSettings);
                    var destinationEndSettings =
                        GetSingleChildElement(destinationEnd, Constants.ElementSpecializationTypes.AssociationEndSettings);

                    var sourceEndAdditionalProperties =
                        GetSingleStereotype(sourceEndSettings, Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Name);
                    var destinationEndAdditionalProperties =
                        GetSingleStereotype(destinationEndSettings, Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Name);

                    return new AssociationSettings
                    {
                        SpecializationType = associationSettingsElement.Name,
                        Icon = GetIcon(associationSettingsElement) ?? _defaultIconModel, // Confirm the default is needed
                        SourceEnd = new AssociationEndSettings
                        {
                            TargetTypes = sourceEndSettings.Attributes
                                .Select(x => x.Type.Element.Name)
                                .ToArray(),
                            IsNavigableEnabled = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableEnabled),
                            IsNullableEnabled = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableEnabled),
                            IsCollectionEnabled = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionEnabled),
                            IsNavigableDefault = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableDefault),
                            IsNullableDefault = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableDefault),
                            IsCollectionDefault = GetSinglePropertyAsBool(sourceEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionDefault),
                        },
                        TargetEnd = new AssociationEndSettings
                        {
                            TargetTypes = destinationEndSettings.Attributes
                            .Select(x => x.Type.Element.Name)
                            .ToArray(),
                            IsNavigableEnabled = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableEnabled),
                            IsNullableEnabled = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableEnabled),
                            IsCollectionEnabled = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionEnabled),
                            IsNavigableDefault = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNavigableDefault),
                            IsNullableDefault = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsNullableDefault),
                            IsCollectionDefault = GetSinglePropertyAsBool(destinationEndAdditionalProperties,
                                Constants.Stereotypes.AssociationEndSettingsAdditionalProperties.Property.IsCollectionDefault),
                        }
                    };
                })
                .ToArray();
        }

        private ElementSettings[] GetElementSettings()
        {
            var items = GetAllChildElementsOfTypes(Model, Constants.ElementSpecializationTypes.ElementSettings)
                .OrderBy(x => x.Name)
                .ToArray();

            return items
                .Select(elementSettingsElement =>
                {
                    var creationOptions =
                        GetElementCreationOptions(GetSingleChildElement(elementSettingsElement, Constants.ElementSpecializationTypes.CreationOptions, true))?
                        .ToArray();

                    var additionalProperties =
                        GetSingleStereotype(elementSettingsElement, Constants.Stereotypes.ElementAdditionalProperties.Name);

                    var attributeSettings =
                        GetSingleChildElement(elementSettingsElement, Constants.ElementSpecializationTypes.AttributeSettings, true)?
                        .ChildElements.Where(x => x.SpecializationType == Constants.ElementSpecializationTypes.AttributeSetting)
                        .Select(GetAttributeSetting)
                        .ToArray();

                    return new ElementSettings
                    {
                        SpecializationType = elementSettingsElement.Name,
                        Icon = GetIcon(elementSettingsElement),
                        ExpandedIcon = GetIcon(elementSettingsElement, expanded: true),
                        AllowRename = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowRename),
                        AllowAbstract = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowAbstract),
                        AllowGenericTypes = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowGenericTypes),
                        AllowMapping = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowMapping),
                        AllowSorting = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowSorting),
                        AllowFindInView = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.ElementAdditionalProperties.Property.AllowFindInView),
                        DiagramSettings = null, // TODO JL
                        LiteralSettings = GetLiteralSettings(elementSettingsElement), // TODO JL
                        AttributeSettings = attributeSettings?.Any() == true
                            ? attributeSettings
                            : null,
                        OperationSettings = null, // TODO JL
                        MappingSettings = null, // TODO JL
                        CreationOptions = creationOptions?.Any() == true
                            ? creationOptions
                            : null,
                        TypeOrder = creationOptions?.Any() == true
                            ? creationOptions.Select(y => y.SpecializationType).ToArray()
                            : null
                    };
                })
                .ToArray();
        }

        private ClassLiteralSettings[] GetLiteralSettings(IElement elementSettingsElement)
        {
            var items = GetAllChildElementsOfTypes(elementSettingsElement, Constants.ElementSpecializationTypes.LiteralSettings)
                .OrderBy(x => x.Name)
                .ToArray();

            if (!items.Any())
            {
                return null;
            }

            return items
                .Select(element =>
                {
                    var additionalProperties = GetSingleStereotype(element, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Name);
                    var text = GetSingleProperty(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.Text);
                    var shortcut = GetSingleProperty(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.Shortcut);
                    var defaultName = GetSingleProperty(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.DefaultName);

                    return new ClassLiteralSettings
                    {
                        SpecializationType = element.Name,
                        Icon = GetIcon(element) ?? _defaultIconModel, // TODO JL: Check if the default actually needed
                        Text = ValueOrFallback(text, $"New {element.Name}"),
                        Shortcut = ValueOrFallback(shortcut, null),
                        DefaultName = ValueOrFallback(defaultName, $"New {element.Name}"),
                        AllowRename = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.AllowRename),
                        AllowDuplicateNames = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.AllowDuplicateNames),
                        AllowFindInView = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.LiteralSettingsAdditionalProperties.Property.AllowFindInView)
                    };
                })
                .ToArray();
        }

        private AttributeSettings GetAttributeSetting(IElement element)
        {
            var additionalProperties = GetSingleStereotype(element, Constants.Stereotypes.AttributeAdditionalProperties.Name);
            var text = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.Text);
            var shortcut = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.Shortcut);
            var displayFunction = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.DisplayFunction);
            var defaultName = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.DefaultName);
            var allowRename = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.AllowRename);
            var allowDuplicateNames = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.AllowDuplicateNames);
            var allowFindInView = GetSinglePropertyAsBool(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.AllowFindInView);
            var defaultTypeId = GetSingleProperty(additionalProperties, Constants.Stereotypes.AttributeAdditionalProperties.Property.DefaultTypeId);

            return new AttributeSettings
            {
                SpecializationType = element.Name,
                Icon = GetIcon(element) ?? _defaultIconModel,
                Text = ValueOrFallback(text, $"New {element.Name}"),
                Shortcut = ValueOrFallback(shortcut, null),
                DisplayFunction = ValueOrFallback(displayFunction, null),
                DefaultName = ValueOrFallback(defaultName, $"New {element.Name}"),
                AllowRename = allowRename,
                AllowDuplicateNames = allowDuplicateNames,
                AllowFindInView = allowFindInView,
                DefaultTypeId = ValueOrFallback(defaultTypeId, null),
                TargetTypes = element.Attributes
                    .Select(x => x.Type.Element.Name)
                    .ToArray()
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

                if (childElement.SpecializationType == Constants.ElementSpecializationTypes.ModelerFolder)
                {
                    foreach (var folderChildElement in GetAllChildElementsOfTypes(childElement, specializationTypes))
                    {
                        yield return folderChildElement;
                    }
                }
            }
        }

        private static string ValueOrFallback(IStereotypeProperty property, string fallback)
        {
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