using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Templates.WebConfig;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    partial class AppSettingsTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.AppSettings";
        private readonly IDictionary<string, ConnectionStringElement> _connectionStrings = new Dictionary<string, ConnectionStringElement>();

        public AppSettingsTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, null)
        {
            eventDispatcher.Subscribe(ApplicationEvents.Config_ConnectionString, HandleConnectionString);
        }

        // GCB - Given that this template is a OnceOff, I don't see the need for the LoadOrCreate. Need to double check.
        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var jsonObject = LoadOrCreate(fullFileName);

            foreach (var connectionString in _connectionStrings.Values)
            {
                var configConnectionStrings = jsonObject["ConnectionStrings"];
                if (configConnectionStrings == null)
                {
                    configConnectionStrings = new JObject();
                    jsonObject["ConnectionStrings"] = configConnectionStrings;
                }
                if (jsonObject["ConnectionStrings"][connectionString.Name] == null)
                {
                    jsonObject["ConnectionStrings"][connectionString.Name] = connectionString.ConnectionString;
                }
            }

            return JsonConvert.SerializeObject(jsonObject, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }

        private dynamic LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName)
                ? JsonConvert.DeserializeObject(File.ReadAllText(fullFileName))
                : JsonConvert.DeserializeObject(TransformText());
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "appsettings",
                fileExtension: "json"
            );
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
    }
}
