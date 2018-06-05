using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.AspNet.WebApi.Templates.HttpExceptionHandler
{
    partial class HttpExceptionHandlerTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IRequiresPreProcessing, IHasAssemblyDependencies
    {
        public const string Identifier = "Intent.AspNet.WebApi.HttpExceptionHandler";

        private readonly IApplicationEventDispatcher _eventDispatcher;

        public HttpExceptionHandlerTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, null)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"ServiceBoundaryExceptionHandlingStrategy",
                fileExtension: "cs",
                defaultLocationInProject: "Providers",
                className: "ServiceBoundaryExceptionHandlingStrategy",
                @namespace: "${Project.Name}"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>()
            {
                NugetPackages.IntentFrameworkCore,
                NugetPackages.IntentFrameworkDomain,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.Container_RegistrationRequired, new Dictionary<string, string>()
            {
                { "InterfaceType", "Intent.Framework.WebApi.ExceptionHandling.IServiceBoundaryExceptionHandlingStrategy"},
                { "ConcreteType", "ServiceBoundaryExceptionHandlingStrategy" }
            });
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http"),
            };
        }
    }
}
