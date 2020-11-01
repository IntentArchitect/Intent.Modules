using System.Collections.Generic;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.UnityServiceProvider
{
    partial class UnityServiceProviderTemplate : CSharpTemplateBase<object>, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Unity.ServiceProvider";

        public UnityServiceProviderTemplate(IProject project)
            : base(Identifier, project, null)
        {
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

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"UnityServiceProvider",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
