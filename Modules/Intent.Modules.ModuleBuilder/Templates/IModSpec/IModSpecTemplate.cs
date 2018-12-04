using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModSpecTemplate : IntentProjectItemTemplateBase<IEnumerable<IClass>>
    {
        public const string TemplateId = "Intent.ModuleBuilder.IModeSpecFile";


        public IModSpecTemplate(string templateId, IProject project, IEnumerable<IClass> models)
            : base(templateId, project, models)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                OverwriteBehaviour.Always,
                CodeGenType.Basic,
                "${Project.ProjectName}",
                "imodspec",
                "");
        }

        public override string TransformText()
        {
            var location = FileMetaData.GetFullLocationPathWithFileName();

            var doc = LoadOrCreateImodSpecFile(location);

            var templatesElement = doc.Element("package").Element("templates");

            templatesElement.RemoveNodes();

            foreach (var model in Model)
            {
                var id = $"{Project.Name}.{model.Name}";
                XElement specificTemplate = templatesElement
                    .Elements("template")
                    .FirstOrDefault(p => p.Attribute("id")?.Value == model.Name);

                if (specificTemplate == null)
                {
                    specificTemplate = new XElement("template", new XAttribute("id", id));
                    templatesElement.Add(specificTemplate);
                }

                var roleElement = specificTemplate.Element("role");
                if (roleElement == null)
                {
                    roleElement = new XElement("role");
                    specificTemplate.Add(roleElement);
                }

                roleElement.Value = id;
            }

            return doc.ToStringUTF8();
        }

        private XDocument LoadOrCreateImodSpecFile(string filePath)
        {
            XDocument doc;
            if (File.Exists(filePath))
            {
                doc = XDocument.Load(filePath);
            }
            else
            {
                doc = XDocument.Parse($@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<package>
  <id>{Project.Name}</id>
  <version>1.0.0</version>
  <summary></summary>
  <description></description>
  <authors></authors>
  <templates>
  </templates>
  <dependencies>
    <dependency id=""Intent.Common"" version=""1.7.0"" />
  </dependencies> 
  <files>
    <file src=""bin\$configuration$\$id$.dll"" />
    <file src=""bin\$configuration$\$id$.pdb"" />
  </files>
</package>");
            }
            return doc;
        }
    }
}
