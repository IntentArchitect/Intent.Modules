using System.Collections.Generic;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.UnityServiceProvider
{
    partial class UnityServiceProviderTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Unity.ServiceProvider";

        public UnityServiceProviderTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        public void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", "IServiceProvider"},
                { "ConcreteType", $"{Namespace}.{ClassName}" },
                { "InterfaceTypeTemplateId", null },
                { "ConcreteTypeTemplateId", Identifier }
            });
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"UnityServiceProvider",
                fileExtension: "cs",
                defaultLocationInProject: "Unity",
                className: $"UnityServiceProvider",
                @namespace: "${Project.Name}.Unity"
                );
        }
    }
}
