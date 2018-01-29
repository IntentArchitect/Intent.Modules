using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Templates.NoT4Template.Templates.Xml
{
    public class Template : Intent.SoftwareFactory.Templates.IntentProjectItemTemplateBase<Dictionary<string, string>>
    {
        public const string Identifier = "Templates.NoT4.Xml";

        public Template(IProject project, Dictionary<string, string> appSettings)
            : base(Identifier, project, appSettings)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "Someoutput",
                fileExtension: "xml",
                defaultLocationInProject: "Templates\\NoT4Template\\Xml"
                );
        }

        public override string TransformText()
        {
            var location = FileMetaData.GetFullLocationPathWithFileName();
            var doc = LoadOrCreate(location);

            var namespaces = new XmlNamespaceManager(new NameTable());
            namespaces.AddNamespace("ns", doc.Root.GetDefaultNamespace().NamespaceName);

            var generatedNode = doc.XPathSelectElement("/ns:someSchema/ns:generatedSettings", namespaces);

            generatedNode.RemoveNodes();

            foreach (var kvp in Model)
            {
                var setting = new XElement(kvp.Key);
                setting.Value = kvp.Value;
                generatedNode.Add(setting);
            }
            return doc.ToStringUTF8();
        }

        private XDocument LoadOrCreate(string filePath)
        {
            XDocument doc;
            if (File.Exists(filePath))
            {
                doc = XDocument.Load(filePath);
            }
            else
            {
                doc = XDocument.Parse($@"
<someSchema>
    <generatedSettings>
    </generatedSettings>
    <manualSettings>
    </manualSettings>
</someSchema>");
            }
            return doc;
        }

    }
}
