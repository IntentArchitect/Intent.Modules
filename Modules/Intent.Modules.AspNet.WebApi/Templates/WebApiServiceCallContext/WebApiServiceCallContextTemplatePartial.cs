using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.AspNet.WebApi.Templates.WebApiServiceCallContext
{
    partial class WebApiServiceCallContextTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies, ITemplateBeforeExecutionHook
    {
        public const string IDENTIFIER = "Intent.AspNet.WebApi.ServiceCallContext";

        public WebApiServiceCallContextTemplate(IProject project)
            : base(IDENTIFIER, project, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"WebApiServiceCallContext",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkCore,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", "Intent.Framework.Core.Context.IContextBackingStore"},
                { "ConcreteType", $"{Namespace}.{ClassName}" }
            });

            Project.Application.EventDispatcher.Publish(InitializationRequiredEvent.EventId, new Dictionary<string, string>()
            {
                { InitializationRequiredEvent.UsingsKey, $@"Intent.Framework.Core.Context;
{Namespace};" },
                { InitializationRequiredEvent.CallKey, "InitializeServiceCallContext();" },
                { InitializationRequiredEvent.MethodKey, $@"
        private void InitializeServiceCallContext()
        {{
            ServiceCallContext.SetBackingStore(new {ClassName}());
        }}" }
            });
        }
    }
}
