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
        public IModeSpecModel(IEnumerable<IDecoratorDefinition> decorators)
        {
            Decorators = decorators;
        }

        public IEnumerable<IDecoratorDefinition> Decorators { get; }
    }

    public class TemplateRegistrationInfo
    {
        public TemplateRegistrationInfo(string templateId, string templateType, string moduleDependency, string moduleVersion)
        {
            TemplateId = templateId;
            TemplateType = templateType;
            ModuleDependency = moduleDependency;
            ModuleVersion = moduleVersion;
        }

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

    public class IModSpecTemplate : IntentProjectItemTemplateBase<IModeSpecModel>, IHasNugetDependencies
    {
        private readonly ICollection<TemplateRegistrationInfo> _templatesToRegister = new List<TemplateRegistrationInfo>();
        private readonly ICollection<MetadataRegistrationInfo> _metadataToRegister = new List<MetadataRegistrationInfo>();

        public const string TemplateId = "Intent.ModuleBuilder.IModeSpecFile";

        public IModSpecTemplate(string templateId, IProject project, IModeSpecModel models)
            : base(templateId, project, models)
        {
            Project.Application.EventDispatcher.Subscribe("TemplateRegistrationRequired", @event =>
            {
                _templatesToRegister.Add(new TemplateRegistrationInfo(@event.GetValue("TemplateId"), @event.GetValue("TemplateType"), @event.TryGetValue("Module Dependency"), @event.TryGetValue("Module Dependency Version")));
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

            foreach (var template in _templatesToRegister)
            {
                var specificTemplate = doc.XPathSelectElement($"package/templates/template[@id=\"{template.TemplateId}\"]");

                if (specificTemplate == null)
                {
                    specificTemplate = new XElement("template", new XAttribute("id", template.TemplateId));
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

                if (specificTemplate.Element("role") == null)
                {
                    specificTemplate.Add(new XElement("role") { Value = template.TemplateId });
                }

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

            if (Model.Decorators.Any())
            {
                var decoratorsElement = doc.Element("package").Element("decorators");
                if (decoratorsElement == null)
                {
                    decoratorsElement = new XElement("decorators");
                    doc.Element("package").Add(decoratorsElement);
                }

                foreach (var model in Model.Decorators)
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

            //foreach (var template in Model.Templates)
            //{
            //    // TODO: GCB - this should probably come from the registration
            //    if (!string.IsNullOrWhiteSpace(template.GetModeler()?.ModuleDependency) && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"{template.GetModeler().ModuleDependency}\"]") == null)
            //    {
            //        var dependencies = doc.XPathSelectElement("package/dependencies");
            //        dependencies.Add(CreateDependency(new IntentModule(template.GetModeler().ModuleDependency, template.GetModeler().ModuleVersion)));
            //    }
            //}

            foreach (var metadataRegistration in _metadataToRegister)
            {
                if (doc.XPathSelectElement($"package/metadata/install[@src=\"{metadataRegistration.Folder}\"]") == null)
                {
                    var metadataRegistrations = doc.XPathSelectElement("package/metadata");
                    metadataRegistrations.Add(new XElement("install", new XAttribute("target", metadataRegistration.Target), new XAttribute("src", metadataRegistration.Folder)));
                }
            }

            //if (Model.Any(x => x.GetModelerName() == "Domain") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.Modelers.Domain\"]") == null)
            //{
            //    var dependencies = doc.XPathSelectElement("package/dependencies");
            //    dependencies.Add(CreateDependency(IntentModule.IntentDomainModeler));
            //}

            //if (Model.Any(x => x.GetModelerName() == "Services") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.Modelers.Services\"]") == null)
            //{
            //    var dependencies = doc.XPathSelectElement("package/dependencies");
            //    dependencies.Add(CreateDependency(IntentModule.IntentServicesModeler));
            //}

            //if (Model.Any(x => x.GetModelerName() == "Eventing") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.Modelers.Eventing\"]") == null)
            //{
            //    var dependencies = doc.XPathSelectElement("package/dependencies");
            //    dependencies.Add(CreateDependency(IntentModule.IntentEventingModeler));
            //}

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
