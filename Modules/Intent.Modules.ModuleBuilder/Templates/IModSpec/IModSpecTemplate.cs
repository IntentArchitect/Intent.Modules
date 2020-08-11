using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Modules.ModuleBuilder.Templates.DesignerSettings;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModeSpecModel
    {
        public IModeSpecModel(IEnumerable<DecoratorModel> decorators)
        {
            Decorators = decorators;
        }

        public IEnumerable<DecoratorModel> Decorators { get; }
    }

    public class TemplateRegistrationInfo
    {
        public TemplateRegistrationInfo(string modelId, string templateId, string templateType, string role, string moduleDependency, string moduleVersion)
        {
            ModelId = modelId;
            TemplateId = templateId;
            TemplateType = templateType;
            Role = role;
            ModuleDependency = moduleDependency;
            ModuleVersion = moduleVersion;
        }

        public string ModelId { get; }
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public string Role { get; }
        public string ModuleDependency { get; }
        public string ModuleVersion { get; }
    }

    public class MetadataRegistrationInfo
    {
        public string Target { get; }
        public string Path { get; }
        public string Id { get; }

        public MetadataRegistrationInfo(string target, string path, string id)
        {
            Target = target;
            Path = CrossPlatformPathHelpers.NormalizePath(path);
            Id = id;
        }
    }

    public class IModSpecTemplate : IntentProjectItemTemplateBase, IHasNugetDependencies
    {
        private readonly IMetadataManager _metadataManager;
        private readonly ICollection<TemplateRegistrationInfo> _templatesToRegister = new List<TemplateRegistrationInfo>();
        private readonly ICollection<MetadataRegistrationRequiredEvent> _metadataToRegister = new List<MetadataRegistrationRequiredEvent>();

        public const string TemplateId = "Intent.ModuleBuilder.IModeSpecFile";

        public IModSpecTemplate(string templateId, IOutputTarget project, IMetadataManager metadataManager)
            : base(templateId, project)
        {
            _metadataManager = metadataManager;
            ExecutionContext.EventDispatcher.Subscribe("TemplateRegistrationRequired", @event =>
            {
                _templatesToRegister.Add(new TemplateRegistrationInfo(
                    modelId: @event.GetValue("ModelId"),
                    templateId: @event.GetValue("TemplateId"),
                    templateType: @event.GetValue("TemplateType"),
                    role: @event.GetValue("Role"),
                    moduleDependency: @event.TryGetValue("Module Dependency"),
                    moduleVersion: @event.TryGetValue("Module Dependency Version")));
            });

            ExecutionContext.EventDispatcher.Subscribe<MetadataRegistrationRequiredEvent>(@event =>
            {
                _metadataToRegister.Add(@event);
            });
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                OverwriteBehaviour.Always,
                CodeGenType.Basic,
                "${Project.ProjectName}",
                "imodspec",
                "");
        }

        public override string TransformText()
        {
            var location = FileMetadata.GetFullLocationPathWithFileName();

            var doc = LoadOrCreateImodSpecFile(location);

            var templatesElement = doc.Element("package").Element("templates");
            if (templatesElement == null)
            {
                templatesElement = new XElement("templates");
                doc.Element("package").Add(templatesElement);
            }

            foreach (var template in _templatesToRegister)
            {
                var specificTemplate = doc.XPathSelectElement($"package/templates/template[@id=\"{template.TemplateId}\"]") ?? doc.XPathSelectElement($"package/templates/template[@externalReference=\"{template.ModelId}\"]");

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

                specificTemplate.Attributes("id").Single().Value = template.TemplateId;
                if (!specificTemplate.Attributes("externalReference").Any())
                {
                    specificTemplate.Add(new XAttribute("externalReference", template.ModelId));
                }

                if (specificTemplate.Element("role") == null)
                {
                    specificTemplate.Add(new XElement("role") { Value = template.Role });
                }
                else
                {
                    specificTemplate.Element("role").Value = template.Role;
                }
                //else
                //{
                //    specificTemplate.Element("role").Value = template.Role;
                //}

                if (!string.IsNullOrWhiteSpace(template.ModuleDependency) && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"{template.ModuleDependency}\"]") == null)
                {
                    var dependencies = doc.XPathSelectElement("package/dependencies");
                    dependencies.Add(CreateDependency(new IntentModule(template.ModuleDependency, template.ModuleVersion)));
                }
            }

            if (_templatesToRegister.Any(x => x.TemplateType == "C# Template") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.OutputManager.RoslynWeaver\"]") == null)
            {
                var dependencies = doc.XPathSelectElement("package/dependencies");
                dependencies.Add(CreateDependency(IntentModule.IntentRoslynWeaver));
            }

            foreach (var template in doc.XPathSelectElements($"package/templates/template"))
            {
                if (template.Attribute("externalReference") != null && _templatesToRegister.All(x => x.ModelId != template.Attribute("externalReference").Value))
                {
                    template.Remove();
                }
            }

            var decorators = _metadataManager.ModuleBuilder(OutputTarget.Application).GetDecoratorModels();
            if (decorators.Any())
            {
                var decoratorsElement = doc.Element("package").Element("decorators");
                if (decoratorsElement == null)
                {
                    decoratorsElement = new XElement("decorators");
                    doc.Element("package").Add(decoratorsElement);
                }

                foreach (var model in decorators)
                {
                    var id = $"{OutputTarget.ApplicationName()}.{model.Name}";
                    var specificDecorator = doc.XPathSelectElement($"package/decorators/decorator[@id=\"{id}\"]");
                    if (specificDecorator == null)
                    {
                        specificDecorator = new XElement("decorator", new XAttribute("id", id));
                        decoratorsElement.Add(specificDecorator);
                    }
                }
            }

            var metadataRegistrations = doc.XPathSelectElement("package/metadata");
            if (metadataRegistrations == null)
            {
                metadataRegistrations = new XElement("metadata");
                doc.XPathSelectElement("package").Add(metadataRegistrations);
            }

            foreach (var metadataRegistration in _metadataToRegister)
            {
                var existing = doc.XPathSelectElement($"package/metadata/install[@src=\"{metadataRegistration.Path}\"]") ?? doc.XPathSelectElement($"package/metadata/install[@externalReference=\"{metadataRegistration.Id}\"]");
                if (existing == null)
                {
                    existing = new XElement("install");
                    metadataRegistrations.Add(existing);
                }
                existing.SetAttributeValue("target", metadataRegistration.Targets.Any()
                        ? string.Join(";", metadataRegistration.Targets.Select(x => x.Name))
                        : null);

                existing.SetAttributeValue("src", metadataRegistration.Path);
                existing.SetAttributeValue("externalReference", metadataRegistration.Id);
            }

            var packagesToInclude = _metadataManager.ModuleBuilder(OutputTarget.Application).GetIntentModuleModels()
                .Where(x => x.GetModuleSettings().IncludeInModule())
                .ToList();
            foreach (var package in packagesToInclude)
            {
                var path = CrossPlatformPathHelpers.NormalizePath(Path.GetRelativePath(GetMetadata().GetFullLocationPath(), package.FileLocation));
                var existing = doc.XPathSelectElement($"package/metadata/install[@src=\"{path}\"]") ?? doc.XPathSelectElement($"package/metadata/install[@externalReference=\"{package.Id}\"]");
                if (existing == null)
                {
                    existing = new XElement("install");
                    metadataRegistrations.Add(existing);
                }

                var targets = package.GetModuleSettings().ReferenceInDesigner();
                existing.SetAttributeValue("target", targets.Any()
                    ? string.Join(";", targets.Select(x => x.Name))
                    : null);

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

        private static XElement CreateDependency(IntentModule intentModule)
        {
            return new XElement(
                "dependency",
                new XAttribute("id", intentModule.Name),
                new XAttribute("version", intentModule.Version));
        }

        private XDocument LoadOrCreateImodSpecFile(string filePath)
        {
            var doc = File.Exists(filePath)
                ? XDocument.Load(filePath)
                : XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<package>
  <id>{OutputTarget.Name}</id>
  <version>1.0.0</version>
  <summary>A custom module for {OutputTarget.Application.SolutionName}.</summary>
  <description>A custom module for {OutputTarget.Application.SolutionName}.</description>
  <authors>{OutputTarget.Application.SolutionName}</authors>
  <templates>
  </templates>
  <dependencies>
    {CreateDependency(IntentModule.IntentCommon)}
    {CreateDependency(IntentModule.IntentCommonTypes)}
  </dependencies>
  <files>
    <file src=""$outDir$/$id$.dll"" />
    <file src=""$outDir$/$id$.pdb"" />
  </files>
</package>");

            return doc;
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.IntentArchitectPackager
            };
        }
    }
}
