using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Settings;
using Intent.Templates;
using Intent.Utils;
using NuGet.Versioning;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModSpecTemplate : IntentFileTemplateBase, IHasNugetDependencies
    {
        private readonly IMetadataManager _metadataManager;
        private readonly ICollection<INugetPackageInfo> _nugetPackages = new List<INugetPackageInfo>();
        private readonly ICollection<TemplateRegistrationRequiredEvent> _templatesToRegister = new List<TemplateRegistrationRequiredEvent>();
        private readonly ICollection<DecoratorRegistrationRequiredEvent> _decoratorsToRegister = new List<DecoratorRegistrationRequiredEvent>();
        private readonly ICollection<FactoryExtensionRegistrationRequiredEvent> _extensionsToRegister = new List<FactoryExtensionRegistrationRequiredEvent>();
        private readonly ICollection<MetadataRegistrationRequiredEvent> _metadataToRegister = new List<MetadataRegistrationRequiredEvent>();
        private readonly ICollection<ModuleDependencyRequiredEvent> _moduleDependencies = new List<ModuleDependencyRequiredEvent>();

        public const string TemplateId = "Intent.ModuleBuilder.IModSpecFile";

        public IModSpecTemplate(IOutputTarget project, IntentModuleModel model, IMetadataManager metadataManager)
            : base(TemplateId, project)
        {
            _metadataManager = metadataManager;
            _nugetPackages.Add(IntentNugetPackages.IntentSdk);
            _nugetPackages.Add(IntentNugetPackages.IntentPackager);
            ModuleModel = model;
            ExecutionContext.EventDispatcher.Subscribe<TemplateRegistrationRequiredEvent>(@event => { _templatesToRegister.Add(@event); });

            ExecutionContext.EventDispatcher.Subscribe<DecoratorRegistrationRequiredEvent>(@event => { _decoratorsToRegister.Add(@event); });

            ExecutionContext.EventDispatcher.Subscribe<FactoryExtensionRegistrationRequiredEvent>(@event => { _extensionsToRegister.Add(@event); });

            _moduleDependencies.Add(new ModuleDependencyRequiredEvent(IntentModule.IntentCommon.Name, IntentModule.IntentCommon.Version));
            _moduleDependencies.Add(new ModuleDependencyRequiredEvent(IntentModule.IntentCommonTypes.Name, IntentModule.IntentCommonTypes.Version));

            foreach (var module in ExecutionContext.InstalledModules.Where(x => x.InstalledMetadataOnly))
            {
                // TODO: GCB - find a way to discover Module Dependencies based on references to the package - these are the actual dependencies (NuGet included)
                _moduleDependencies.Add(new ModuleDependencyRequiredEvent(module.ModuleId, module.Version));
            }

            ExecutionContext.EventDispatcher.Subscribe<ModuleDependencyRequiredEvent>(@event =>
            {
                if (@event.ModuleId == ModuleModel.Name)
                {
                    return;
                }

                _moduleDependencies.Add(@event);
            });

            ExecutionContext.EventDispatcher.Subscribe<MetadataRegistrationRequiredEvent>(@event => { _metadataToRegister.Add(@event); });
        }

        public IntentModuleModel ModuleModel { get; }


        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: ModuleModel.Name,
                fileExtension: "imodspec");
        }

        public override string TransformText()
        {
            var content = TryGetExistingFileContent(out var existingContent)
                ? existingContent
                : CreateImodSpecFile();

            var doc = XDocument.Parse(content);

            /*--------------------------------- HEADER METADATA ---------------------------------*/

            doc.Element("package").SetElementValue("id", ModuleModel.Name);

            if (NuGetVersion.TryParse(doc.Element("package").Element("version")?.Value, out var moduleVersion) &&
                NuGetVersion.TryParse(ModuleModel.Version, out var versionFromDesigner) &&
                versionFromDesigner > moduleVersion)
            {
                doc.Element("package").SetElementValue("version", ModuleModel.Version);
            }

            if (doc.Element("package").Element("supportedClientVersions") != null)
            {
                // update 4.0.0 constraint to 5.0.0 because SDK (and V4) is essentially backward compatible
                doc.Element("package").SetElementValue("supportedClientVersions", doc.Element("package").Element("supportedClientVersions").Value
                    .Replace("[3.3.0", "[4.1.0")
                    .Replace("[3.4.0", "[4.1.0")
                    .Replace("[4.1.0-a,", "[4.1.0-beta.21,")
                    .Replace("[4.1.0-beta.21,", "[4.2.3,")
                    .Replace("[4.1.0,", "[4.3.0-a,")
                    .Replace("[4.2.3,", "[4.3.0-a,")
                    .Replace("4.0.0)", "5.0.0)"));
            }

            if (ExecutionContext.GetApplicationConfig().Description != null)
            {
                doc.Element("package").SetElementValue("summary", ExecutionContext.GetApplicationConfig().Description);
                doc.Element("package").SetElementValue("description", ExecutionContext.GetApplicationConfig().Description);
            }

            var tagsElement = doc.Element("package").Element("tags");
            if (tagsElement == null)
            {
                tagsElement = new XElement("tags");
                doc.Element("package").Add(tagsElement);
            }

            var releaseNotesElement = doc.Element("package").Element("releaseNotes");
            if (releaseNotesElement == null && ModuleModel.GetModuleSettings().IncludeReleaseNotes())
            {
                releaseNotesElement = new XElement("releaseNotes", "release-notes.md");
                doc.Element("package").Add(releaseNotesElement);
            }

            var projectUrlElement = doc.Element("package").Element("projectUrl");
            if (projectUrlElement == null && !string.IsNullOrWhiteSpace(ModuleModel.GetModuleSettings().ProjectURL()))
            {
                projectUrlElement = new XElement("projectUrl", ModuleModel.GetModuleSettings().ProjectURL());
                doc.Element("package").Add(projectUrlElement);
            }

            var templatesElement = doc.Element("package").Element("templates");
            if (templatesElement == null)
            {
                templatesElement = new XElement("templates");
                doc.Element("package").Add(templatesElement);
            }

            var icon = doc.XPathSelectElement($"package/iconUrl");
            if (icon == null)
            {
                icon = new XElement("iconUrl");
                doc.Element("package").Add(icon);
            }

            icon.SetValue(OutputTarget.Application.Icon.Source);

            /*--------------------------------- DESIGN ELEMENTS ---------------------------------*/

            foreach (var template in _templatesToRegister)
            {
                var specificTemplate = doc.XPathSelectElement($"package/templates/template[@externalReference=\"{template.ModelId}\"]");

                // backward compatibility
                if (specificTemplate == null)
                {
                    var sameId = doc.XPathSelectElement($"package/templates/template[@id=\"{template.TemplateId}\"]");
                    specificTemplate = sameId?.Attribute("externalReference") == null ? sameId : null;
                }

                if (specificTemplate == null)
                {
                    specificTemplate = new XElement("template", new XAttribute("id", template.TemplateId), new XAttribute("externalReference", template.ModelId));
                    if (template.TemplateType == "C# Template")
                    {
                        specificTemplate.Add(XElement.Parse(@"
      <config>
        <add key=""ClassName"" description=""Class name formula override (e.g. '${Model.Name}')"" />
        <add key=""Namespace"" description=""Class namespace formula override (e.g. '${Project.Name}'"" />
      </config>"));
                    }

                    templatesElement.Add(specificTemplate);
                }

                specificTemplate.SetAttributeValue("id", template.TemplateId);
                if (!specificTemplate.Attributes("externalReference").Any())
                {
                    specificTemplate.Add(new XAttribute("externalReference", template.ModelId));
                }

                specificTemplate.SetElementValue("role", template.Role ?? string.Empty);
                specificTemplate.SetElementValue("location", template.Location ?? string.Empty);
            }

            SortChildElementsByAttribute(templatesElement, "id");

            var factoryExtensions = doc.Element("package").Element("factoryExtensions");
            if (_extensionsToRegister.Any() && factoryExtensions == null)
            {
                factoryExtensions = new XElement("factoryExtensions");
                doc.Element("package").Add(factoryExtensions);
            }

            foreach (var extension in _extensionsToRegister)
            {
                var specificExtension = doc.XPathSelectElement($"package/factoryExtensions/factoryExtension[@externalReference=\"{extension.ModelId}\"]");

                if (specificExtension == null)
                {
                    specificExtension = new XElement("factoryExtension", new XAttribute("id", extension.ExtensionId), new XAttribute("externalReference", extension.ModelId));

                    factoryExtensions.Add(specificExtension);
                }

                specificExtension.SetAttributeValue("id", extension.ExtensionId);
                if (!specificExtension.Attributes("externalReference").Any())
                {
                    specificExtension.Add(new XAttribute("externalReference", extension.ModelId));
                }
            }

            SortChildElementsByAttribute(factoryExtensions, "id");

            foreach (var extension in doc.XPathSelectElements($"package/factoryExtensions/factoryExtension").ToList())
            {
                if (extension.Attribute("externalReference") != null && _extensionsToRegister.All(x => x.ModelId != extension.Attribute("externalReference").Value))
                {
                    extension.Remove();
                }
            }

            if (_templatesToRegister.Any(x => x.TemplateType == "C# Template"))
            {
                _moduleDependencies.Add(new ModuleDependencyRequiredEvent(IntentModule.IntentRoslynWeaver.Name, IntentModule.IntentRoslynWeaver.Version));
            }

            foreach (var template in doc.XPathSelectElements($"package/templates/template").ToList())
            {
                if (template.Attribute("externalReference") != null && _templatesToRegister.All(x => x.ModelId != template.Attribute("externalReference").Value))
                {
                    template.Remove();
                }
            }

            /*--------------------------------- MODULE DEPENDENCIES ---------------------------------*/

            var dependencies = doc.XPathSelectElement("package/dependencies");
            foreach (var moduleDependency in _moduleDependencies)
            {
                if (moduleDependency.ModuleId == doc.XPathSelectElement("package/id").Value)
                {
                    continue;
                }

                var existing = doc.XPathSelectElement($"package/dependencies/dependency[@id=\"{moduleDependency.ModuleId}\"]");
                if (existing == null)
                {
                    existing = CreateDependency(new IntentModule(moduleDependency.ModuleId, moduleDependency.ModuleVersion));
                    dependencies.Add(existing);

                    existing.SetAttributeValue("version", moduleDependency.ModuleVersion);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(existing.Attribute("version").Value) ||
                    ExecutionContext.Settings.GetModuleBuilderSettings()?.DependencyVersionOverwriteBehavior()?.IsAlways() == true ||
                    (
                        (ExecutionContext.Settings.GetModuleBuilderSettings()?.DependencyVersionOverwriteBehavior() == null ||
                         ExecutionContext.Settings.GetModuleBuilderSettings().DependencyVersionOverwriteBehavior().IsIfNewer()) &&
                        NuGetVersion.TryParse(existing.Attribute("version").Value, out var version) && version < NuGetVersion.Parse(moduleDependency.ModuleVersion)
                    ))
                {
                    existing.SetAttributeValue("version", moduleDependency.ModuleVersion);
                }
            }

            SortChildElementsByAttribute(dependencies, "id");

            /*--------------------------------- DECORATORS ---------------------------------*/

            var decoratorsElement = doc.Element("package").Element("decorators");
            if (_decoratorsToRegister.Any())
            {
                if (decoratorsElement == null)
                {
                    decoratorsElement = new XElement("decorators");
                    doc.Element("package").Add(decoratorsElement);
                }

                foreach (var decorator in _decoratorsToRegister)
                {
                    var specificDecorator = doc.XPathSelectElement($"package/decorators/decorator[@externalReference=\"{decorator.ModelId}\"]");
                    if (specificDecorator == null)
                    {
                        specificDecorator = new XElement("decorator", new XAttribute("id", decorator.DecoratorId), new XAttribute("externalReference", decorator.ModelId));
                        decoratorsElement.Add(specificDecorator);
                    }

                    specificDecorator.SetAttributeValue("id", decorator.DecoratorId);
                }
            }

            SortChildElementsByAttribute(decoratorsElement, "id");

            foreach (var decorator in doc.XPathSelectElements($"package/decorators/decorator").ToList())
            {
                if (decorator.Attribute("externalReference") != null && _decoratorsToRegister.All(x => x.ModelId != decorator.Attribute("externalReference").Value))
                {
                    decorator.Remove();
                }
            }

            /*--------------------------------- MODULE SETTINGS ---------------------------------*/

            var moduleSettings = doc.XPathSelectElement("package/moduleSettings");
            if (moduleSettings == null)
            {
                moduleSettings = new XElement("moduleSettings");
                doc.XPathSelectElement("package").Add(moduleSettings);
            }

            foreach (var settingsGroup in ModuleModel.SettingsGroups)
            {
                var existing = doc.XPathSelectElement($"package/moduleSettings/group[@id=\"{settingsGroup.Id}\"]");
                if (existing == null)
                {
                    existing = new XElement("group", new XAttribute("id", settingsGroup.Id), new XAttribute("title", settingsGroup.Name),
                        new XAttribute("externalReference", settingsGroup.Id));
                    moduleSettings.Add(existing);
                }

                existing.SetAttributeValue("title", settingsGroup.Name);

                var settings = new XElement("settings");
                foreach (var settingsField in settingsGroup.Fields)
                {
                    var fieldXml = new XElement("setting",
                        new XAttribute("id", settingsField.Id),
                        new XAttribute("title", settingsField.Name),
                        new XAttribute("type", GetControlType(settingsField)));
                    if (settingsField.GetFieldConfiguration().IsRequired())
                    {
                        fieldXml.Add(new XElement("isRequired", new XText(settingsField.GetFieldConfiguration().IsRequired().ToString().ToLower())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().Hint()))
                    {
                        fieldXml.Add(new XElement("hint", new XText(settingsField.GetFieldConfiguration().Hint())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().DefaultValue()))
                    {
                        fieldXml.Add(new XElement("defaultValue", new XText(settingsField.GetFieldConfiguration().DefaultValue())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().IsActiveFunction()))
                    {
                        fieldXml.Add(new XElement("isActiveFunction", new XText(settingsField.GetFieldConfiguration().IsActiveFunction())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().IsRequiredFunction()))
                    {
                        fieldXml.Add(new XElement("isRequiredFunction", new XText(settingsField.GetFieldConfiguration().IsRequiredFunction())));
                    }

                    if (settingsField.GetFieldConfiguration().ControlType().IsSelect() ||
                        settingsField.GetFieldConfiguration().ControlType().IsMultiSelect())
                    {
                        var options = new XElement("options");
                        foreach (var fieldOption in settingsField.Options)
                        {
                            options.Add(new XElement("option", new XAttribute("value", fieldOption.Value ?? fieldOption.Name), new XAttribute("description", fieldOption.Name)));
                        }

                        fieldXml.Add(options);
                    }

                    settings.Add(fieldXml);
                }

                if (existing.HasElements)
                {
                    existing.RemoveNodes();
                }

                existing.Add(settings);
            }

            SortChildElementsByAttribute(moduleSettings, "id");

            foreach (var installedSettingsGroup in doc.XPathSelectElements($"package/moduleSettings/group"))
            {
                if (installedSettingsGroup.Attribute("externalReference") != null &&
                    ModuleModel.SettingsGroups.All(x => x.Id != installedSettingsGroup.Attribute("externalReference").Value))
                {
                    installedSettingsGroup.Remove();
                }
            }

            /*--------------------------------- MODULE SETTINGS EXTENSIONS ---------------------------------*/

            var moduleSettingsExtensions = doc.XPathSelectElement("package/moduleSettingsExtensions");
            if (moduleSettingsExtensions == null)
            {
                moduleSettingsExtensions = new XElement("moduleSettingsExtensions");
                doc.XPathSelectElement("package").Add(moduleSettingsExtensions);
            }

            foreach (var groupExtension in ModuleModel.SettingsExtensions)
            {
                var existing = doc.XPathSelectElement($"package/moduleSettingsExtensions/groupExtension[@id=\"{groupExtension.Id}\"]");
                if (existing == null)
                {
                    existing = new XElement("groupExtension", new XAttribute("id", groupExtension.Id), new XAttribute("groupId", groupExtension.TypeReference.Element.Id),
                        new XAttribute("externalReference", groupExtension.Id));
                    moduleSettingsExtensions.Add(existing);
                }

                existing.SetAttributeValue("groupId", groupExtension.TypeReference.Element.Id);

                var settings = new XElement("settings");
                foreach (var settingsField in groupExtension.Fields)
                {
                    var fieldXml = new XElement("setting",
                        new XAttribute("id", settingsField.Id),
                        new XAttribute("title", settingsField.Name),
                        new XAttribute("type", GetControlType(settingsField)));
                    fieldXml.Add(new XElement("isRequired", new XText(settingsField.GetFieldConfiguration().IsRequired().ToString().ToLower())));
                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().Hint()))
                    {
                        fieldXml.Add(new XElement("hint", new XText(settingsField.GetFieldConfiguration().Hint())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().DefaultValue()))
                    {
                        fieldXml.Add(new XElement("defaultValue", new XText(settingsField.GetFieldConfiguration().DefaultValue())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().IsActiveFunction()))
                    {
                        fieldXml.Add(new XElement("isActiveFunction", new XText(settingsField.GetFieldConfiguration().IsActiveFunction())));
                    }

                    if (!string.IsNullOrWhiteSpace(settingsField.GetFieldConfiguration().IsRequiredFunction()))
                    {
                        fieldXml.Add(new XElement("isRequiredFunction", new XText(settingsField.GetFieldConfiguration().IsRequiredFunction())));
                    }

                    if (settingsField.GetFieldConfiguration().ControlType().IsSelect() ||
                        settingsField.GetFieldConfiguration().ControlType().IsMultiSelect())
                    {
                        var options = new XElement("options");
                        foreach (var fieldOption in settingsField.Options)
                        {
                            options.Add(new XElement("option", new XAttribute("value", fieldOption.Value ?? fieldOption.Name), new XAttribute("description", fieldOption.Name)));
                        }

                        fieldXml.Add(options);
                    }

                    settings.Add(fieldXml);
                }

                if (existing.HasElements)
                {
                    existing.RemoveNodes();
                }

                existing.Add(settings);
            }

            SortChildElementsByAttribute(moduleSettingsExtensions, "id");

            foreach (var installedExtension in doc.XPathSelectElements($"package/moduleSettingsExtensions/groupExtension"))
            {
                if (installedExtension.Attribute("externalReference") != null &&
                    ModuleModel.SettingsExtensions.All(x => x.Id != installedExtension.Attribute("externalReference").Value))
                {
                    installedExtension.Remove();
                }
            }

            /*--------------------------------- METADATA ---------------------------------*/

            var metadataRegistrations = doc.XPathSelectElement("package/metadata");
            if (metadataRegistrations == null)
            {
                metadataRegistrations = new XElement("metadata");
                doc.XPathSelectElement("package").Add(metadataRegistrations);
            }

            foreach (var metadataRegistration in _metadataToRegister)
            {
                var existing = doc.XPathSelectElement($"package/metadata/install[@src=\"{metadataRegistration.Path}\"]") ??
                               doc.XPathSelectElement($"package/metadata/install[@externalReference=\"{metadataRegistration.Id}\"]");
                if (existing == null)
                {
                    existing = new XElement("install");
                    metadataRegistrations.Add(existing);
                }

                existing.SetAttributeValue("target", metadataRegistration.Targets.Any()
                    ? string.Join(";", metadataRegistration.Targets.Select(x => x.Name))
                    : null);

                existing.SetAttributeValue("src", PathHelper.GetRelativePath(GetMetadata().GetFullLocationPath(), metadataRegistration.Path).NormalizePath());
                existing.SetAttributeValue("externalReference", metadataRegistration.Id);
            }


            var notIncludedModules = _metadataManager.ModuleBuilder(OutputTarget.Application).StereotypeDefinitions
                .Where(x => x.Package.SpecializationTypeId == IntentModuleModel.SpecializationTypeId &&
                            !new IntentModuleModel(x.Package).GetModuleSettings().IncludeInModule())
                .Select(x => new IntentModuleModel(x.Package))
                .ToList();
            foreach (var notIncludedModule in notIncludedModules)
            {
                Logging.Log.Warning(
                    $"Intent Module [{notIncludedModule.Name}] package has Stereotype Definitions but is not set to be included. Check your Module Settings in the Module Builder designer.");
            }

            var packagesToInclude = _metadataManager.ModuleBuilder(OutputTarget.Application).GetIntentModuleModels()
                .Where(x => x.GetModuleSettings().IncludeInModule())
                .ToList();
            foreach (var package in packagesToInclude)
            {
                if (package.GetModuleSettings().ReferenceIn()?.IsAllDesigners() != true &&
                    package.GetModuleSettings().ReferenceInDesigner()?.Any() != true)
                {
                    Logging.Log.Warning(
                        $"Intent Module [{package.Name}] package is included but not set to be automatically referenced in a designer. Check your Module Settings in the Module Builder designer.");
                }

                var path = PathHelper.GetRelativePath(GetMetadata().GetFullLocationPath(), package.FileLocation).NormalizePath();
                var existing = doc.XPathSelectElement($"package/metadata/install[@src=\"{path}\"]") ??
                               doc.XPathSelectElement($"package/metadata/install[@externalReference=\"{package.Id}\"]");
                if (existing == null)
                {
                    existing = new XElement("install");
                    metadataRegistrations.Add(existing);
                }

                var referenceInDesigners = package.GetModuleSettings().ReferenceInDesigner();
                var referenceIn = package.GetModuleSettings().ReferenceIn();
                var target = referenceIn switch
                {
                    _ when referenceIn.IsAllDesigners() => "*",
                    _ when referenceInDesigners != null => string.Join(";", referenceInDesigners.Select(x => x.Name)),
                    _ => null
                };
                existing.SetAttributeValue("target", target);

                existing.SetAttributeValue("src", path);
                existing.SetAttributeValue("externalReference", package.Id);
            }

            var managedInstalls = _metadataToRegister.Select(x => x.Id).Concat(packagesToInclude.Select(x => x.Id)).ToList();
            foreach (var installElements in doc.XPathSelectElements($"package/metadata/install"))
            {
                if (installElements.Attribute("externalReference") != null && managedInstalls.All(x => x != installElements.Attribute("externalReference").Value))
                {
                    installElements.Remove();
                }
            }

            return doc.ToStringUTF8();
        }

        private static void SortChildElementsByAttribute(XElement element, string attributeName)
        {
            if (element == null || !element.HasElements)
            {
                return;
            }

            var sortedElements = element.Elements().OrderBy(x => x.Attribute(attributeName).Value).ToArray();
            element.RemoveNodes();
            element.Add(sortedElements);
        }

        private static XElement CreateDependency(IntentModule intentModule)
        {
            return new XElement(
                "dependency",
                new XAttribute("id", intentModule.Name),
                new XAttribute("version", intentModule.Version));
        }

        private string CreateImodSpecFile()
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<package>
  <id>{ModuleModel.Name}</id>
  <version>{ModuleModel.GetModuleSettings().Version()}</version>
  <supportedClientVersions>[4.1.0-beta.21,5.0.0)</supportedClientVersions>
  <summary>{ExecutionContext.GetApplicationConfig().Description ?? $"A custom module for {OutputTarget.Application.SolutionName}"}.</summary>
  <description>{ExecutionContext.GetApplicationConfig().Description ?? $"A custom module for {OutputTarget.Application.SolutionName}"}.</description>
  <authors>{OutputTarget.Application.SolutionName}</authors>
  <iconUrl></iconUrl>
  <templates></templates>
  <decorators></decorators>
  <factoryExtensions></factoryExtensions>
  <moduleSettings></moduleSettings>
  <dependencies></dependencies>
  <files>
    <file src=""$outDir$/$id$.dll"" />
    <file src=""$outDir$/$id$.pdb"" />
  </files>
</package>";
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return _nugetPackages;
        }

        private string GetControlType(ModuleSettingsFieldConfigurationModel settingsField)
        {
            if (settingsField.GetFieldConfiguration().ControlType().IsTextBox())
            {
                return "text";
            }

            if (settingsField.GetFieldConfiguration().ControlType().IsNumber())
            {
                return "number";
            }

            if (settingsField.GetFieldConfiguration().ControlType().IsCheckbox())
            {
                return "checkbox";
            }

            if (settingsField.GetFieldConfiguration().ControlType().IsSwitch())
            {
                return "switch";
            }

            if (settingsField.GetFieldConfiguration().ControlType().IsTextArea())
            {
                return "textarea";
            }

            if (settingsField.GetFieldConfiguration().ControlType().IsSelect())
            {
                return "select";
            }

            if (settingsField.GetFieldConfiguration().ControlType().IsMultiSelect())
            {
                return "multi-select";
            }

            return "text";
        }
    }
}