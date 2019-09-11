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
    public class IModSpecTemplate : IntentProjectItemTemplateBase<IEnumerable<IModuleBuilderElement>>, IHasNugetDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.IModeSpecFile";


        public IModSpecTemplate(string templateId, IProject project, IEnumerable<IModuleBuilderElement> models)
            : base(templateId, project, models)
        {
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

            foreach (var model in Model.Where(p => p.Type == ModuleBuilderElementType.CSharpTemplate || p.Type == ModuleBuilderElementType.FileTemplate))
            {
                var id = $"{Project.ApplicationName()}.{model.Name}";
                var specificTemplate = doc.XPathSelectElement($"package/templates/template[@id=\"{id}\"]");

                if (specificTemplate == null)
                {
                    specificTemplate = new XElement("template", new XAttribute("id", id));
                    templatesElement.Add(specificTemplate);
                }

                if (specificTemplate.Element("role") == null)
                {
                    specificTemplate.Add(new XElement("role") { Value = id });
                }
            }

            if (Model.Any(p => p.Type == ModuleBuilderElementType.Decorator))
            {
                var decoratorsElement = doc.Element("package").Element("decorators");
                if (decoratorsElement == null)
                {
                    decoratorsElement = new XElement("decorators");
                    doc.Element("package").Add(decoratorsElement);
                }

                foreach (var model in Model.Where(p => p.Type == ModuleBuilderElementType.Decorator))
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

            if (Model.Any(x => x.Type == ModuleBuilderElementType.CSharpTemplate) && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.OutputManager.RoslynWeaver\"]") == null)
            {
                var dependencies = doc.XPathSelectElement("package/dependencies");
                dependencies.Add(CreateDependency(IntentModule.IntentRoslynWeaver));
            }

            if (Model.Any(x => x.GetModelerName() == "Domain") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.Modelers.Domain\"]") == null)
            {
                var dependencies = doc.XPathSelectElement("package/dependencies");
                dependencies.Add(CreateDependency(IntentModule.IntentDomainModeler));
            }

            if (Model.Any(x => x.GetModelerName() == "Services") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.Modelers.Services\"]") == null)
            {
                var dependencies = doc.XPathSelectElement("package/dependencies");
                dependencies.Add(CreateDependency(IntentModule.IntentServicesModeler));
            }

            if (Model.Any(x => x.GetModelerName() == "Eventing") && doc.XPathSelectElement($"package/dependencies/dependency[@id=\"Intent.Modelers.Eventing\"]") == null)
            {
                var dependencies = doc.XPathSelectElement("package/dependencies");
                dependencies.Add(CreateDependency(IntentModule.IntentEventingModeler));
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
