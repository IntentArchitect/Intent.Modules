using System.IO;
using System.Xml.Linq;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    partial class CoreWebCSProjectTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.CSProject";

        public CoreWebCSProjectTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        // GCB - Given that this template is a OnceOff, I don't see the need for the LoadOrCreate. Need to double check.
        public override string RunTemplate()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var doc = LoadOrCreate(fullFileName);
            return doc.ToString();
        }

        private XDocument LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName)
                ? XDocument.Load(fullFileName)
                : XDocument.Parse(TransformText(), LoadOptions.PreserveWhitespace);
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "csproj",
                defaultLocationInProject: ""
            );
        }
    }
}
