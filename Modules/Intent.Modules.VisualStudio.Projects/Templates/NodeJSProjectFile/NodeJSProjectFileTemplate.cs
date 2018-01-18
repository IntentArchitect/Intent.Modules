using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Microsoft.Build.Construction;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.NodeJSProjectFile
{
    public class NodeJSProjectFileTemplate : IntentProjectItemTemplateBase<object>, IProjectTemplate, ISupportXmlDecorators
    {
        public const string Identifier = "Intent.VisualStudio.Projects.NodeJSProjectFile";

        private readonly Dictionary<string, IXmlDecorator> _decorators = new Dictionary<string, IXmlDecorator>();

        public NodeJSProjectFileTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "njsproj",
                defaultLocationInProject: ""
                );
        }

        public override string TransformText()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var doc = LoadOrCreate(fullFileName);
            foreach (var decorator in GetDecorators())
            {
                decorator.Install(doc, Project);
            }
            return doc.ToString();
        }

        private XDocument LoadOrCreate(string fullFileName)
        {
            XDocument doc;
            if (File.Exists(fullFileName))
            {
                doc = System.Xml.Linq.XDocument.Load(fullFileName);
            }
            else
            {
                doc = System.Xml.Linq.XDocument.Parse(CreateTemplate());
            }
            return doc;
        }

        private IEnumerable<IXmlDecorator> GetDecorators()
        {
            //May need to bring in application / project level decorators too
            return _decorators.Values;
        }

        public void RegisterDecorator(string id, IXmlDecorator decorator)
        {
            if (!_decorators.ContainsKey(id))
            {
                _decorators.Add(id, decorator);
            }

        }

        public string CreateTemplate()
        {
            var root = ProjectRootElement.Create();
            root.ToolsVersion = "4.0";
            root.DefaultTargets = "Build";

            var group = root.AddPropertyGroup();
            group.AddProperty("VisualStudioVersion", "11.0").Condition = "'$(VisualStudioVersion)' == ''";
            group.AddProperty("VSToolsPath", @"$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)").Condition = "'$(VSToolsPath)' == ''";
            group.AddProperty("Name", $"{Project.Name}");
            group.AddProperty("RootNamespace", $"{Project.Name}");

            root.AddImport(@"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props").Condition = @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')";

            group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug").Condition = " '$(Configuration)' == '' ";
            group.AddProperty("SchemaVersion", "2.0");
            group.AddProperty("ProjectGuid", $"{{{Project.Id}}}");
            group.AddProperty("ProjectHome", ".");
            group.AddProperty("StartWebBrowser", "False");
            group.AddProperty("WorkingDirectory", ".");
            group.AddProperty("OutputPath", ".");
            group.AddProperty("OutputType", "Library");
            group.AddProperty("TargetFrameworkVersion", Project.TargetFrameworkVersion());
            group.AddProperty("ProjectTypeGuids", "{3AF33F2E-1136-4D97-BBB7-1795711AC8B8};{9092AA53-FB77-4645-B42D-1CCCA6BD08BD}");
            group.AddProperty("ProjectView", "ShowAllFiles");
            group.AddProperty("TypeScriptSourceMap", "true");
            group.AddProperty("TypeScriptModuleKind", "CommonJS");
            group.AddProperty("EnableTypeScript", "true");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)' == 'Debug' ";
            group.AddProperty("DebugSymbols", "true");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)' == 'Release' ";
            group.AddProperty("DebugSymbols", "true");

            root.AddImport(@"$(VSToolsPath)\Node.js Tools\Microsoft.NodejsTools.targets");

            return root.RawXml.Replace("utf-16", "utf-8");
        }
    }
}