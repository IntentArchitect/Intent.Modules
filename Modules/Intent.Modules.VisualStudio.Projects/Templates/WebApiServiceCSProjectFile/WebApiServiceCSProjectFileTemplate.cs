using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebApiServiceCSProjectFile
{
    public class WebApiServiceCSProjectFileTemplate : IntentProjectItemTemplateBase<object>, IHasNugetDependencies, ISupportXmlDecorators, IHasDecorators<ICSProjectFileDecorator>, IProjectTemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.WebApiServiceCSProjectFile";
        private readonly Dictionary<string, IXmlDecorator> _xmlDecorators = new Dictionary<string, IXmlDecorator>();
        private readonly string _sslPort = "";
        private readonly string _port;
        private IEnumerable<ICSProjectFileDecorator> _decorators;

        public WebApiServiceCSProjectFileTemplate(IProject project)
            : base(Identifier, project, null)
        {
            _port = project.ProjectType.Properties.First(x => x.Name == "Port").Value;
            bool useSsl;
            bool.TryParse(project.ProjectType.Properties.First(x => x.Name == "UseSsl").Value, out useSsl);
            if (useSsl)
            {
                _sslPort = project.ProjectType.Properties.First(x => x.Name == "SslPort").Value;
            }
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

        public override string TransformText()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var doc = LoadOrCreate(fullFileName);
            foreach (var decorator in GetXmlDecorators())
            {
                decorator.Install(doc, Project);
            }
            return doc.ToStringUTF8();

        }

        private XDocument LoadOrCreate(string fullFileName)
        {
            XDocument doc;
            if (File.Exists(fullFileName))
            {
                doc = XDocument.Load(fullFileName);
            }
            else
            {
                doc = XDocument.Parse(CreateTemplate());
            }
            return doc;
        }

        public IEnumerable<ICSProjectFileDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private IEnumerable<IXmlDecorator> GetXmlDecorators()
        {
            return _xmlDecorators.Values.Union(GetDecorators());
        }

        // TODO: ISupportXmlDecorators and GetXmlDecorators probably shouldn't be here, so far as I can see nothing is relying on it
        public void RegisterDecorator(string id, IXmlDecorator decorator)
        {
            if (!_xmlDecorators.ContainsKey(id))
            {
                _xmlDecorators.Add(id, decorator);
            }
        }

        public string CreateTemplate()
        {
            var root = ProjectRootElement.Create();
            root.ToolsVersion = "12.0";
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
            group.AddProperty("TargetFrameworkVersion", Project.TargetFrameworkVersion());
            group.AddProperty("WcfConfigValidationEnabled", "True");
            group.AddProperty("AutoGenerateBindingRedirects", "true");
            group.AddProperty("UseIISExpress", "True");
            group.AddProperty("IISExpressSSLPort", _sslPort);
            group.AddProperty("IISExpressAnonymousAuthentication", "");
            group.AddProperty("IISExpressWindowsAuthentication", "");
            group.AddProperty("IISExpressUseClassicPipelineMode", "");
            group.AddProperty("UseGlobalApplicationHostFile", "");
            group.AddProperty("NuGetPackageImportStamp", "");
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
                , "System.Data"
                , "System.Data.DataSetExtensions"
                , "System.Drawing"
                , "System.EnterpriseServices"
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
            root.AddImport(@"$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v10.0\WebApplications\Microsoft.WebApplication.targets").Condition = "false";

            group = root.AddPropertyGroup();
            group.AddProperty("VisualStudioVersion", "10.0").Condition = "'$(VisualStudioVersion)' == ''";
            group.AddProperty("VSToolsPath", @"$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)").Condition = "'$(VSToolsPath)' == ''";

            if (!string.IsNullOrEmpty(_sslPort))
            {
                var extension = root.CreateProjectExtensionsElement();
                extension.Content =
                    $@"    <VisualStudio>
      <FlavorProperties GUID=""{{349c5851-65df-11da-9384-00065b846f21}}"">
        <WebProjectProperties>
          <UseIIS>True</UseIIS>
          <AutoAssignPort>{ (!string.IsNullOrWhiteSpace(_port) ? "False" : "True") }</AutoAssignPort>
          <DevelopmentServerPort>{ _port ?? "0" }</DevelopmentServerPort>
          <DevelopmentServerVPath>/</DevelopmentServerVPath>
          <IISUrl>https://localhost:{  _sslPort }/</IISUrl>
          <NTLMAuthentication>False</NTLMAuthentication>
          <UseCustomServer>False</UseCustomServer>
          <CustomServerUrl>
          </CustomServerUrl>
          <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>
        </WebProjectProperties>
      </FlavorProperties>
    </VisualStudio>
";
                root.AppendChild(extension);
            }

            return root.RawXml.Replace("utf-16", "utf-8");
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

        private static void AddItem(ProjectItemGroupElement itemGroup, string groupName, string item, IEnumerable<KeyValuePair<string, string>> metaData)
        {
            itemGroup.AddItem(groupName, item, metaData);
        }

        private void AddReference(ProjectItemGroupElement itemGroup, IAssemblyReference reference)
        {
            var metaData = new List<KeyValuePair<string, string>>();
            if (reference.HasHintPath())
            {
                metaData.Add(new KeyValuePair<string, string>("HintPath", reference.HintPath));
            }
            AddItem(itemGroup, "Reference", reference.Library, metaData);
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkCore,
                NugetPackages.MicrosoftAspNetWebApi,
                NugetPackages.MicrosoftAspNetWebApiClient,
                NugetPackages.MicrosoftAspNetWebApiCore,
                NugetPackages.MicrosoftAspNetWebApiWebHost,
                NugetPackages.NewtonsoftJson,
            };
        }
    }
}