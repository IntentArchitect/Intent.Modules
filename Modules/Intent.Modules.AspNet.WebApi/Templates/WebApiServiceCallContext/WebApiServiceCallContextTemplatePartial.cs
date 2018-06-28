using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Templates.WebApiServiceCallContext
{
    partial class WebApiServiceCallContextTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.AspNet.WebApi.ServiceCallContext";

        public WebApiServiceCallContextTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
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

        public void PreProcess()
        {
            Project.Application.EventDispatcher.Publish(ContainerRegistration.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", $"Intent.Framework.Core.Context.IContextBackingStore"},
                { "ConcreteType", $"{Namespace}.{ClassName}" }
            });
        }
    }
}
