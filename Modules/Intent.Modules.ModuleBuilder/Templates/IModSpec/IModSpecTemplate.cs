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
        public TemplateRegistrationInfo(string modelId, string templateId, string templateType, string moduleDependency, string moduleVersion)
        {
            ModelId = modelId;
            TemplateId = templateId;
            TemplateType = templateType;
            ModuleDependency = moduleDependency;
            ModuleVersion = moduleVersion;
        }

        public string ModelId { get; }
        public string TemplateId { get; set; }
        public string TemplateType { get; set; }
        public string ModuleDependency { get; }
        public string ModuleVersion { get; }
    }

    public class MetadataRegistrationInfo
    {
        public string Target { get; }
        public string Folder { get; }

        public MetadataRegistrationInfo(string target, string folder)
        {
            Target = target;
            Folder = folder;
        }
    }

    public class IModSpecTemplate : IntentProjectItemTemplateBase<object>, IHasNugetDependencies
    {
        private readonly IMetadataManager _metadataManager;
        private readonly ICollection<TemplateRegistrationInfo> _templatesToRegister = new List<TemplateRegistrationInfo>();
        private readonly ICollection<MetadataRegistrationInfo> _metadataToRegister = new List<MetadataRegistrationInfo>();

        public const string TemplateId = "Intent.ModuleBuilder.IModeSpecFile";

        public IModSpecTemplate(string templateId, IProject project, IMetadataManager metadataManager)
            : base(templateId, project, null)
        {
            _metadataManager = metadataManager;
            Project.Application.EventDispatcher.Subscribe("TemplateRegistrationRequired", @event =>
            {
                _templatesToRegister.Add(new TemplateRegistrationInfo(@event.GetValue("ModelId"), @event.GetValue("TemplateId"), @event.GetValue("TemplateType"), @event.TryGetValue("Module Dependency"), @event.TryGetValue("Module Dependency Version")));
            });

            Project.Application.EventDispatcher.Subscribe("MetadataRegistrationRequired", @event =>
            {
                _metadataToRegister.Add(new MetadataRegistrationInfo(@event.GetValue("Target"), @event.GetValue("Folder")));
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
                    specificTemplate.Add(new XElement("role") { Value = template.TemplateId });
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

            // TODO: Test
            foreach (var template in doc.XPathSelectElements($"package/templates/template"))
            {
                if (template.Attribute("externalReference") != null && _templatesToRegister.All(x => x.ModelId != template.Attribute("externalReference").Value))
                {
                    template.Remove();
                }
            }

            var decorators = _metadataManager.GetDecoratorModels(Project.Application).ToList();
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
                    var id = $"{Project.ApplicationName()}.{model.Name}";
                    var specificDecorator = doc.XPathSelectElement($"package/decorators/decorator[@id=\"{id}\"]");
                    if (specificDecorator == null)
                    {
                        specificDecorator = new XElement("decorator", new XAttribute("id", id));
                        decoratorsElement.Add(specificDecorator);
                    }
                }
            }

            foreach (var metadataRegistration in _metadataToRegister)
            {
                if (doc.XPathSelectElement($"package/metadata/install[@src=\"{metadataRegistration.Folder}\"]") == null)
                {
                    var metadataRegistrations = doc.XPathSelectElement("package/metadata");
                    metadataRegistrations.Add(new XElement("install", new XAttribute("target", metadataRegistration.Target), new XAttribute("src", metadataRegistration.Folder)));
                }
            }

            var packagesToInclude = _metadataManager.GetMetadata<IPackage>("Module Builder", Project.Application.Id)
                .Where(x => x.GetStereotypeProperty("Package Settings", "Include in Module", false))
                .ToList();
            foreach (var package in packagesToInclude)
            {
                var path = CrossPlatformPathHelpers.NormalizePath(Path.GetRelativePath(GetMetadata().GetFullLocationPath(), Path.GetDirectoryName(package.FileLocation)));
                if (path.StartsWith(".."))
                {
                    throw new Exception($"Package [{package.Name}] cannot be included because it is not within the Module Builder root location [{GetMetadata().GetFullLocationPath()}]");
                }
                if (doc.XPathSelectElement($"package/metadata/install[@src=\"{path}\"]") == null)
                {
                    var metadataRegistrations = doc.XPathSelectElement("package/metadata");
                    metadataRegistrations.Add(new XElement("install",
                        new XAttribute("target", package.GetStereotypeProperty<IElement>("Package Settings", "Reference in Designer")?.Name ?? string.Empty),
                        new XAttribute("src", path)));
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
  <id>{Project.Name}</id>
  <version>1.0.0</version>
  <summary>A custom module for {Project.Application.SolutionName}.</summary>
  <description>A custom module for {Project.Application.SolutionName}.</description>
  <authors>{Project.Application.SolutionName}</authors>
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
