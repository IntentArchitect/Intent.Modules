using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Npm.Config.Templates.PackageJsonConfig
{
    public class PackageJsonTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Npm.Config.PackageJson";

        public PackageJsonTemplate(IProject project, IApplicationEventDispatcher applicationEventDispatcher)
            : base(Identifier, project, null)
        {
            applicationEventDispatcher.Subscribe(ApplicationEvents.Typescript_TypingsRequired, Handle);
        }

        public IDictionary<string, string> DevDependencies { get; } = new Dictionary<string, string>();

        private void Handle(ApplicationEvent @event)
        {
            DevDependencies.Add(@event.GetValue("name"), @event.GetValue("version"));
        }

        public override string TransformText()
        {
            dynamic config;
            if (!File.Exists(GetMetaData().GetFullLocationPathWithFileName()))
            {
                config = new PackageJsonFile()
                {
                    name = Project.ApplicationName(),
                    version = "1.0.0",
                    @private = true,
                    devDependencies = JObject.FromObject(new Dictionary<string, string>())
                };
            }
            else
            {
                var existingFileContent = File.ReadAllText(GetMetaData().GetFullLocationPathWithFileName());
                config = JsonConvert.DeserializeObject(existingFileContent, new JsonSerializerSettings());
            }

            if (config.devDependencies == null)
            {
                config.devDependencies = JObject.FromObject(new Dictionary<string, string>());
            }

            foreach (var devDependency in DevDependencies)
            {
                if (config.devDependencies[devDependency.Key] == null)
                {
                    config.devDependencies[devDependency.Key] = devDependency.Value;
                }
            }

            return JsonConvert.SerializeObject(config, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "package",
                fileExtension: "json",
                defaultLocationInProject: ""
                );
        }
    }

    public class PackageJsonFile
    {
        public string version { get; set; }
        public string name { get; set; }
        public bool @private { get; set; }
        public object devDependencies { get; set; }
    }
}