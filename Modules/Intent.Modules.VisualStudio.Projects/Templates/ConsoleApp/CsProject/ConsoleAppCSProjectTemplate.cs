using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.CsProject
{
    public class ConsoleAppCsProjectTemplate : VisualStudioProjectTemplateBase
    {
        public const string Identifier = "Intent.VisualStudio.Projects.ConsoleApp.CSProject";

        public ConsoleAppCsProjectTemplate(IProject project, IVisualStudioProject model)
            : base (Identifier, project, model)
        {
        }

        public override string TransformText()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var doc = LoadOrCreate(fullFileName);
            return doc.ToStringUTF8();
        }

        private XDocument LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName)
                ? XDocument.Load(fullFileName)
                : XDocument.Parse(CreateTemplate());
        }

        private string CreateTemplate()
        {
            var root = ProjectRootElement.Create();
            root.ToolsVersion = "14.0";
            root.DefaultTargets = "Build";

            var group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug").Condition = " '$(Configuration)' == '' ";
            group.AddProperty("Platform", "AnyCPU").Condition = " '$(Platform)' == '' ";
            group.AddProperty("ProjectGuid", $"{{{Model.Id}}}");
            group.AddProperty("OutputType", "Exe");
            group.AddProperty("AppDesignerFolder", "Properties");
            group.AddProperty("RootNamespace", $"{Model.Name}");
            group.AddProperty("AssemblyName", $"{Model.Name}");
            group.AddProperty("TargetFrameworkVersion", GetTargetFrameworkVersion());
            group.AddProperty("FileAlignment", "512");
            group.AddProperty("TargetFrameworkProfile", "");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
            group.AddProperty("DebugSymbols", "true");
            group.AddProperty("DebugType", "full");
            group.AddProperty("Optimize", "false");
            group.AddProperty("OutputPath", "bin\\Debug\\");
            group.AddProperty("DefineConstants", "DEBUG;TRACE");
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
            group.AddProperty("DebugType", "pdbonly");
            group.AddProperty("Optimize", "true");
            group.AddProperty("OutputPath", "bin\\Release\\");
            group.AddProperty("DefineConstants", "TRACE");
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");

            var itemGroup = AddItems(root, "Reference", 
                "Microsoft.CSharp"
                , "System"
                , "System.Core"
                , "System.Data.DataSetExtensions"
                , "System.Data"
                , "System.Xml"
                , "System.Xml.Linq");

            foreach (var reference in Project.References())
            {
                AddReference(itemGroup, reference);
            }

            foreach (var dependency in Project.Dependencies())
            {
                AddItem(itemGroup, "ProjectReference", string.Format("..\\{0}\\{0}.csproj", dependency.Name),
                    new[]
                    {
                        new KeyValuePair<string, string>("Project", $"{{{dependency.Id}}}"),
                        new KeyValuePair<string, string>("Name", $"{dependency.Name}"),
                    });
            }

            root.AddImport("$(MSBuildToolsPath)\\Microsoft.CSharp.targets");

            return root.RawXml.Replace("utf-16", "utf-8");
        }

        private string GetTargetFrameworkVersion()
        {
            return Model.TargetFrameworkVersion().SingleOrDefault() ?? "4.7.2";
        }

        private static ProjectItemGroupElement AddItems(ProjectRootElement elem, string groupName, params string[] items)
        {
            var group = elem.AddItemGroup();
            foreach (var item in items)
            {
                group.AddItem(groupName, item);
            }
            return group;
        }

        private static void AddItem(ProjectItemGroupElement itemGroup, string groupName, string item, IEnumerable<KeyValuePair<string, string>> metadata)
        {
            itemGroup.AddItem(groupName, item, metadata);
        }

        private static void AddReference(ProjectItemGroupElement itemGroup, IAssemblyReference reference)
        {
            var metadata = new List<KeyValuePair<string, string>>();
            if (reference.HasHintPath())
            {
                metadata.Add(new KeyValuePair<string, string>("HintPath", reference.HintPath));
            }
            AddItem(itemGroup, "Reference", reference.Library, metadata);
        }
    }
}