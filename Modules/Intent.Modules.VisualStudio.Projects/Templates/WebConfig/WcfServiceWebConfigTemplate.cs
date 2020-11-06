/*
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebConfig
{
    public class WcfServiceWebConfigTemplate : IntentFileTemplateBase<object>, ITemplate, IHasDecorators<IWebConfigDecorator>
    {
        private IEnumerable<IWebConfigDecorator> _decorators;

        public WcfServiceWebConfigTemplate(IProject project)
            : base(CoreTemplateId.WcfServiceWebConfig, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: "Web",
                fileExtension: "config",
                relativeLocation: ""
                );
        }

        public override string TransformText()
        {
            var location = FileMetadata.GetFullLocationPathWithFileName();

            var doc = LoadOrCreateWebConfig(location);
            foreach (var webConfigDecorator in GetDecorators())
            {
                webConfigDecorator.Install(doc, Project);
            }
            return doc.ToStringUTF8();
        }

        public IEnumerable<IWebConfigDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private static XDocument LoadOrCreateWebConfig(string filePath)
        {
            XDocument doc;
            if (File.Exists(filePath))
            {
                doc = XDocument.Load(filePath);
            }
            else
            {
                doc = XDocument.Parse(@"<?xml version=""1.0""?>
<configuration>

  <appSettings>
    <add key=""aspnet:UseTaskFriendlySynchronizationContext"" value=""true"" />
  </appSettings>

  <system.web>
    <compilation debug=""true"" targetFramework=""4.5"" />
    <httpRuntime targetFramework=""4.5""/>
  </system.web>

  <system.serviceModel>
    <behaviors>
      <serviceBehaviors>
        <behavior>
          <!-- To avoid disclosing metadata information, set the values below to false before deployment -->
          <serviceMetadata httpGetEnabled=""true"" httpsGetEnabled=""true""/>
          <!-- To receive exception details in faults for debugging purposes, set the value below to true.  Set to false before deployment to avoid disclosing exception information -->
          <serviceDebug includeExceptionDetailInFaults=""false""/>
        </behavior>
      </serviceBehaviors>
    </behaviors>
    <protocolMapping>
        <add binding=""basicHttpsBinding"" scheme=""https"" />
    </protocolMapping>    
    <serviceHostingEnvironment aspNetCompatibilityEnabled=""true"" multipleSiteBindingsEnabled=""true"" />
  </system.serviceModel>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests=""true""/>
    <!--
        To browse web app root directory during debugging, set the value below to true.
        Set to false before deployment to avoid disclosing web app folder information.
      -->
    <directoryBrowse enabled=""true""/>
  </system.webServer>

</configuration>");
            }
            return doc;
        }
    }
}
*/