using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.WebApi.Templates.WebApiServiceCallContext
{
    partial class WebApiServiceCallContextTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.AspNetCore.WebApi.ServiceCallContext";

        public WebApiServiceCallContextTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "WebApiServiceCallContext",
                fileExtension: "cs",
                defaultLocationInProject: "Context",
                className: "WebApiServiceCallContext",
                @namespace: "${Project.ProjectName}.Context"
                );
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
                { "InterfaceType", $"Intent.Framework.Core.Context.IContextBackingStore"},
                { "ConcreteType", $"{Namespace}.{ClassName}" }
            });
        }
    }
}
