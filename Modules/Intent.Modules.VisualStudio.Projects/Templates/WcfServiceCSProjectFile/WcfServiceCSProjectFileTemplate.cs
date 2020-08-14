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

namespace Intent.Modules.VisualStudio.Projects.Templates.WcfServiceCSProjectFile
{
    public class WcfServiceCSProjectFileTemplate : VisualStudioProjectTemplateBase, IHasNugetDependencies
    {
        public const string IDENTIFIER = "Intent.VisualStudio.Projects.WcfServiceCSProjectFile";

        public WcfServiceCSProjectFileTemplate(IOutputTarget project, IVisualStudioProject model)
            : base(IDENTIFIER, project, model)
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

            root.AddImport(@"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props").Condition = "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')";

            var group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug").Condition = " '$(Configuration)' == '' ";
            group.AddProperty("Platform", "AnyCPU").Condition = " '$(Platform)' == '' ";
            group.AddProperty("ProductVersion", "");
            group.AddProperty("SchemaVersion", "2.0");
            group.AddProperty("ProjectGuid", Project.Id.ToString());
            group.AddProperty("ProjectTypeGuids", "{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}");
            group.AddProperty("OutputType", "Library");
            group.AddProperty("AppDesignerFolder", "Properties");
            group.AddProperty("RootNamespace", Project.Name);
            group.AddProperty("AssemblyName", Project.Name);
            group.AddProperty("TargetFrameworkVersion", GetTargetFrameworkVersion());
            group.AddProperty("WcfConfigValidationEnabled", "True");
            group.AddProperty("AutoGenerateBindingRedirects", "true");
            group.AddProperty("UseIISExpress", "True");
            group.AddProperty("IISExpressSSLPort", "");
            group.AddProperty("IISExpressAnonymousAuthentication", "");
            group.AddProperty("IISExpressWindowsAuthentication", "");
            group.AddProperty("IISExpressUseClassicPipelineMode", "");
            group.AddProperty("UseGlobalApplicationHostFile", "");
            group.AddProperty("TargetFrameworkProfile", "");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
            group.AddProperty("DebugSymbols", "true");
            group.AddProperty("DebugType", "full");
            group.AddProperty("Optimize", "false");
            group.AddProperty("OutputPath", @"bin\");
            group.AddProperty("DefineConstants", "DEBUG;TRACE");
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");

            group = root.AddPropertyGroup();
            group.Condition = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
            group.AddProperty("DebugType", "pdbonly");
            group.AddProperty("Optimize", "true");
            group.AddProperty("OutputPath", @"bin\");
            group.AddProperty("DefineConstants", "TRACE");
            group.AddProperty("ErrorReport", "prompt");
            group.AddProperty("WarningLevel", "4");

            // references
            var itemGroup = AddItems(root, "Reference"
                , "Microsoft.CSharp"
                , "System"
                , "System.Configuration"
                , "System.Core"
                , "System.Data"
                , "System.Drawing"
                , "System.EnterpriseServices"
                , "System.Runtime.Serialization"
                , "System.ServiceModel"
                , "System.ServiceModel.Web"
                , "System.Transactions"
                , "System.Web"
                , "System.Web.ApplicationServices"
                , "System.Web.DynamicData"
                , "System.Web.Entity"
                , "System.Web.Extensions"
                , "System.Web.Services"
                , "System.Xml"
                , "System.Xml.Linq"
                );

            itemGroup = root.AddItemGroup();

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

            root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
            root.AddImport(@"$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets").Condition = "'$(VSToolsPath)' != ''";

            group = root.AddPropertyGroup();
            group.AddProperty("VisualStudioVersion", "10.0").Condition = "'$(VisualStudioVersion)' == ''";
            group.AddProperty("VSToolsPath", @"$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)").Condition = "'$(VSToolsPath)' == ''";

            var extensions = root.CreateProjectExtensionsElement();
            extensions.Content = "None";
            //        extensions.Content =
            //            @"  <VisualStudio>
            //<FlavorProperties GUID=""{349c5851-65df-11da-9384-00065b846f21}"">
            //    <WebProjectProperties>
            //      <UseIIS>True</UseIIS>
            //      <AutoAssignPort>True</AutoAssignPort>
            //      <DevelopmentServerPort>52670</DevelopmentServerPort>
            //      <DevelopmentServerVPath>/</DevelopmentServerVPath>
            //      <IISUrl>http://localhost:52670/</IISUrl>
            //      <NTLMAuthentication>False</NTLMAuthentication>
            //      <UseCustomServer>False</UseCustomServer>
            //      <CustomServerUrl>
            //      </CustomServerUrl>
            //      <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>
            //    </WebProjectProperties>
            //  </FlavorProperties>
            //</VisualStudio>";

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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[0];
        }
    }
}