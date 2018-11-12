using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    partial class CoreLibraryCSProjectTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IProjectTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreLibrary.CSProject";

        public CoreLibraryCSProjectTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        // GCB - Given that this template is a OnceOff, I don't see the need for the LoadOrCreate. Need to double check.
        public override string RunTemplate()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var doc = LoadOrCreate(fullFileName);

            Project.InstallNugetPackages(doc);
            //var nugetPackages = Project
            //    .NugetPackages()
            //    .GroupBy(x => x.Name)
            //    .ToDictionary(x => x.Key, x => x);

            //var packageReferenceItemGroup = doc.XPathSelectElement("Project/ItemGroup[PackageReference]");
            //if (packageReferenceItemGroup == null)
            //{
            //    packageReferenceItemGroup = new XElement("ItemGroup");
            //    doc.XPathSelectElement("Project").Add(packageReferenceItemGroup);
            //}

            //foreach (var addFileBehaviour in nugetPackages)
            //{
            //    var existingReference = packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{addFileBehaviour.Key}']");
            //    if (existingReference == null)
            //    {
            //        packageReferenceItemGroup.Add(new XElement("PackageReference", new XAttribute("Include", addFileBehaviour.Key), new XAttribute("Version", addFileBehaviour.Value.OrderByDescending(x => x.Version).First().Version)));
            //    }
            //}

            return doc.ToString();
        }

        private XDocument LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName)
                ? XDocument.Load(fullFileName, LoadOptions.PreserveWhitespace)
                : XDocument.Parse(TransformText(), LoadOptions.PreserveWhitespace);
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "csproj",
                defaultLocationInProject: ""
            );
        }
    }
}
