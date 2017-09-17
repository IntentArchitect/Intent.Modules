using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Intent.Modules.AngularJs.Shell.Templates.AngularConfigJs
{
    partial class AngularConfigJsTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.AngularJs.Shell.ConfigJs";
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public AngularConfigJsTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            _eventDispatcher = eventDispatcher;
            eventDispatcher.Subscribe(ApplicationEvents.AngularJs_ConfigurationRequired, Handle);
        }

        public override string RunTemplate()
        {
            string fileName = FileMetaData.GetFullLocationPathWithFileName();
            if (File.Exists(fileName))
            {
                var result = new StringBuilder();
                result.Append(@"//IntentManaged[configs]" + Environment.NewLine);
                result.Append(string.Join(Environment.NewLine, ConfigItems.Select(x => $"    {x.Key}: \"{x.Value}\",")));
                result.Append(Environment.NewLine + @"//IntentManaged[configs]" + Environment.NewLine);
                return result.ToString();
            }
            else
            {
                return TransformText();
            }
        }

        private void Handle(ApplicationEvent @event)
        {
            if (ConfigItems.ContainsKey(@event.GetValue("Key")))
            {
                if (ConfigItems[@event.GetValue("Key")] != @event.GetValue("Value"))
                {
                    throw new Exception($"Misconfiguration in [{GetType().Name}]: Config with key [{@event.GetValue("Key")}] already defined with different value to [{@event.GetValue("Value")}].");
                }
                return;
            }

            ConfigItems.Add(@event.GetValue("Key"), @event.GetValue("Value"));
            ConfigItems = ConfigItems.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public IDictionary<string, string> ConfigItems { get; private set; } = new Dictionary<string, string>();

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledTagWeave,
                fileName: "config",
                fileExtension: "js",
                defaultLocationInProject: "wwwroot/js"
                );
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.IndexHtml_JsFileAvailable, new Dictionary<string, string>() { { "Src", "./js/config.js" } });
        }
    }
}
