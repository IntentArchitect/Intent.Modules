using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebConfig
{
    public class WebApiWebConfigFileTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasDecorators<IWebConfigDecorator>
    {
        private IEnumerable<IWebConfigDecorator> _decorators;
        private readonly IDictionary<string, string> _appSettings = new Dictionary<string, string>();
        private readonly IDictionary<string, ConnectionStringElement> _connectionStrings = new Dictionary<string, ConnectionStringElement>();

        public WebApiWebConfigFileTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(CoreTemplateId.WebApiWebConfig, project, null)
        {
            eventDispatcher.Subscribe(ApplicationEvents.Config_AppSetting, HandleAppSetting);
            eventDispatcher.Subscribe(ApplicationEvents.Config_ConnectionString, HandleConnectionString);
        }

        private void HandleAppSetting(ApplicationEvent @event)
        {
            if (_appSettings.ContainsKey(@event.GetValue("Key")))
            {
                if (_appSettings[@event.GetValue("Key")] != @event.GetValue("Value"))
                {
                    // TODO: Do not commit
                    //throw new Exception($"Misconfiguration in [{GetType().Name}]: AppSetting with key [{@event.Key}] already defined with different value to [{@event.Value}].");
                }
                return;
            }
            _appSettings.Add(@event.GetValue("Key"), @event.GetValue("Value"));
        }

        private void HandleConnectionString(ApplicationEvent @event)
        {
            if (_connectionStrings.ContainsKey(@event.GetValue("Name")))
            {
                if (_connectionStrings[@event.GetValue("Name")].ConnectionString != @event.GetValue("ConnectionString"))
                {
                    throw new Exception($"Misconfiguration in [{GetType().Name}]: ConnectionString with name [{@event.GetValue("Name")}] already defined with different value to [{@event.GetValue("ConnectionString")}].");
                }
                return;
            }
            _connectionStrings.Add(@event.GetValue("Name"), new ConnectionStringElement(name: @event.GetValue("Name"), connectionString: @event.GetValue("ConnectionString"), providerName: @event.GetValue("ProviderName")));
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: "Web",
                fileExtension: "config",
                defaultLocationInProject: ""
                );
        }

        public override string TransformText()
        {
            var location = FileMetaData.GetFullLocationPathWithFileName();

            var doc = LoadOrCreateWebConfig(location);

            var namespaces = new XmlNamespaceManager(new NameTable());
            namespaces.AddNamespace("ns", doc.Root.GetDefaultNamespace().NamespaceName);

            foreach (var appSetting in _appSettings)
            {
                var configAppSettings = doc.XPathSelectElement("/ns:configuration/ns:appSettings", namespaces);
                if (configAppSettings.XPathSelectElement($"//add[@key='{appSetting.Key}']", namespaces) == null)
                {
                    var setting = new XElement("add");
                    setting.Add(new XAttribute("key", appSetting.Key));
                    setting.Add(new XAttribute("value", appSetting.Value));
                    configAppSettings.Add(setting);
                }
            }

            foreach (var connectionString in _connectionStrings.Values)
            {
                var configConnectionStrings = doc.XPathSelectElement("/ns:configuration/ns:connectionStrings", namespaces);
                if (configConnectionStrings == null)
                {
                    configConnectionStrings = new XElement("connectionStrings");
                    var configAppSettings = doc.XPathSelectElement("/ns:configuration/ns:appSettings", namespaces);
                    if (configAppSettings == null)
                    {
                        doc.Root.AddFirst(configConnectionStrings);
                    }
                    else
                    {
                        configAppSettings.AddBeforeSelf(configConnectionStrings);
                    }
                }
                if (configConnectionStrings.XPathSelectElement($"//add[@name='{connectionString.Name}']", namespaces) == null)
                {
                    var setting = new XElement("add");
                    setting.Add(new XAttribute("name", connectionString.Name));
                    setting.Add(new XAttribute("providerName", connectionString.ProviderName));
                    setting.Add(new XAttribute("connectionString", connectionString.ConnectionString));
                    configConnectionStrings.Add(setting);
                }
            }
            foreach (var webConfigDecorator in GetDecorators())
            {
                webConfigDecorator.Install(doc, Project);
            }
            return doc.ToStringUTF8();
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
                doc = XDocument.Parse(@"<!--
  For more information on how to configure your ASP.NET application, please visit
  http://go.microsoft.com/fwlink/?LinkId=301879
  -->
<configuration>
  <configSections>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
  </configSections>
  <appSettings>
  </appSettings>
  <system.web>
    <compilation debug=""true"" targetFramework=""4.5.2"" />
    <httpRuntime targetFramework=""4.5.2"" />
  </system.web>
  <system.webServer>
    <handlers>
      <remove name=""ExtensionlessUrlHandler-Integrated-4.0"" />
      <remove name=""OPTIONSVerbHandler"" />
      <remove name=""TRACEVerbHandler"" />
      <add name=""ExtensionlessUrlHandler-Integrated-4.0"" path=""*."" verb=""*"" type=""System.Web.Handlers.TransferRequestHandler"" preCondition=""integratedMode,runtimeVersionv4.0"" />
    </handlers>
  </system.webServer>
  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
    </assemblyBinding>
  </runtime>
</configuration>");
            }
            return doc;
        }

        public IEnumerable<IWebConfigDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }
    }

    public class ConnectionStringElement
    {
        public ConnectionStringElement(string name, string connectionString, string providerName)
        {
            Name = name;
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
    }
}
