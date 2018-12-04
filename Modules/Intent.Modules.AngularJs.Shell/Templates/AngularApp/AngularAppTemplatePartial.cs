using Intent.Modules.Bower.Contracts;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AngularJs.Shell.Templates.AngularApp
{
    partial class AngularAppTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasBowerDependencies, IRequiresPreProcessing
    {
        private readonly IApplicationEventDispatcher _applicationEventDispatcher;
        public const string Identifier = "Intent.AngularJs.Shell.AngularApp";

        public AngularAppTemplate(IProject project, IApplicationEventDispatcher applicationEventDispatcher) 
            : base(Identifier, project, null)
        {
            _applicationEventDispatcher = applicationEventDispatcher;
            applicationEventDispatcher.Subscribe(ApplicationEvents.AngularJs_ModuleRegistered, Handle);
        }

        public ISet<string> AngularModules { get; } = new HashSet<string>();

        private void Handle(ApplicationEvent @event)
        {
            AngularModules.Add(@event.GetValue("ModuleName"));
        }

        public override string RunTemplate()
        {
            string fileName = FileMetaData.GetFullLocationPathWithFileName();
            if (File.Exists(fileName))
            {
                var result = new StringBuilder();
                result.Append(@"//IntentManaged[modules]" + Environment.NewLine);
                result.Append(string.Join(Environment.NewLine,  AngularModules.Select(x => $"\t\t\t, \"{x}\"")));
                result.Append(Environment.NewLine + @"//IntentManaged[modules]" + Environment.NewLine);
                return result.ToString();
            }
            else
            {
                return TransformText();
            }
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledTagWeave,
                fileName: "App",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App"
                );
        }

        public IEnumerable<IBowerPackageInfo> GetBowerDependencies()
        {
            return new List<IBowerPackageInfo>
            {
                BowerPackages.JQuery,
                BowerPackages.Bootstrap,
                BowerPackages.Angular,
                BowerPackages.AngularUiRouter,
            };
        }

        public void PreProcess()
        {
            _applicationEventDispatcher.Publish(ApplicationEvents.Typescript_TypingsRequired, new Dictionary<string, string>()
            {
                { "name", "@types/angular" },
                { "version", "^1.6.2" }
            });
            _applicationEventDispatcher.Publish(ApplicationEvents.Typescript_TypingsRequired, new Dictionary<string, string>()
            {
                { "name", "@types/angular-ui-router" },
                { "version", "^1.1.36" }
            });
        }
    }
}
