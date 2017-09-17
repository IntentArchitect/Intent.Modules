using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService
{
    public partial class WebApiEventConsumerServiceTemplate : IntentRoslynProjectItemTemplateBase<SubscribingModel>, ITemplate, IHasNugetDependencies, IHasDecorators<IEventConsumerDecorator>, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.Messaging.Subscriber.Controller.Legacy";
        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IEnumerable<IEventConsumerDecorator> _decorators;

        public WebApiEventConsumerServiceTemplate(IProject project, SubscribingModel model, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, model)
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
                fileName: $"EventConsumerServiceController",
                fileExtension: "cs",
                defaultLocationInProject: "Controllers\\Generated"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>
            {
                NugetPackages.IntentEsbClient,
            }
            .Union(base.GetNugetDependencies())
            .ToList();
        }

        public string DeclareUsings()
        {
            return GetDecorators().Aggregate(x => x.DeclareUsings()
                .Distinct()
                .Select(y => $"{Environment.NewLine}{y}")
                .OrderBy(y => y)
                .DefaultIfEmpty()
                .Aggregate((y, z) => $"{y}{z}"));
        }

        public string DeclarePrivateVariables()
        {
            return GetDecorators().Aggregate(x => x.DeclarePrivateVariables());
        }

        public string ConstructorParams()
        {
            return GetDecorators().Aggregate(x => x.ConstructorParams());
        }

        public string ConstructorInit()
        {
            return GetDecorators().Aggregate(x => x.ConstructorInit());
        }

        public string BeginOperation()
        {
            return GetDecorators().Aggregate(x => x.BeginOperation());
        }

        public string BeforeTransaction()
        {
            return GetDecorators().Aggregate(x => x.BeforeTransaction());
        }

        public string BeforeCallToAppLayer()
        {
            return GetDecorators().Aggregate(x => x.BeforeCallToAppLayer());
        }

        public string AfterCallToAppLayer()
        {
            return GetDecorators().Aggregate(x => x.AfterCallToAppLayer());
        }

        public string AfterTransaction()
        {
            return GetDecorators().Aggregate(x => x.AfterTransaction());
        }

        public string OnExceptionCaught()
        {
            var onExceptionCaught = GetDecorators().Aggregate(x => x.OnExceptionCaught());

            if (GetDecorators().Any(x => x.HandlesCaughtException()))
            {
                return onExceptionCaught;
            }

            return onExceptionCaught + @"
                throw;";
        }

        public IEnumerable<IEventConsumerDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish("Config.ConnectionString", new Dictionary<string, string>()
            {
                { "Name", $"{Project.ApplicationName()}DB" },
                { "ConnectionString", $"Server=.;Initial Catalog={ Project.Application.SolutionName };Integrated Security=true;MultipleActiveResultSets=True" },
                { "ProviderName", "System.Data.SqlClient" },
            });
        }
    }
}
