using Intent.Modules.Bower.Contracts;
using Intent.Engine;
using Intent.Templates
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Templates;
using SearchOption = Intent.SoftwareFactory.Engine.SearchOption;

namespace Intent.Modules.Bower.Templates.BowerConfig
{
    public class BowerConfigTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Bower.BowerConfig";
        public BowerConfigTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override string TransformText()
        {
            bool outputChanged = false;
            BowerConfig config;
            string existingFileContent = null;

            if (!File.Exists(GetMetaData().GetFullLocationPathWithFileName()))
            {
                config = new BowerConfig();
                outputChanged = true;
            }
            else
            {
                existingFileContent = File.ReadAllText(GetMetaData().GetFullLocationPathWithFileName());
                config = JsonConvert.DeserializeObject<BowerConfig>(existingFileContent, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }

            var bowerPackages = Project.FindTemplateInstances(null, x => x is IHasBowerDependencies, SearchOption.AllProjects).Cast<IHasBowerDependencies>()
                .SelectMany(x => x.GetBowerDependencies())
                .Distinct()
                .ToList();

            foreach (var bowerPackage in bowerPackages)
            {
                if (config.Dependencies.Keys.Any(x => x == bowerPackage.Name))
                {
                    continue;
                }
                config.Dependencies.Add(bowerPackage.Name, bowerPackage.Version);
                outputChanged = true;
            }

            if (!outputChanged)
            {
                return existingFileContent;
            }
            return JsonConvert.SerializeObject(config, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "bower",
                fileExtension: "json",
                defaultLocationInProject: ""
                );
        }
    }

    public class BowerConfig
    {
        public string Name { get; set; } = "ASP.NET";
        public bool Private { get; set; } = true;
        public Dictionary<string, string> Dependencies { get; set; } = new Dictionary<string, string>();
    }
}
