using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.Eventing;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.Templates

namespace Intent.Modules.Messaging.Subscriber.LegacyCodeBasedDsl.Templates.WebApiEventConsumerService
{
    public partial class WebApiEventConsumerServiceTemplate : IntentRoslynProjectItemTemplateBase<SubscribingModel>, ITemplate, IHasNugetDependencies, IHasDecorators<WebApiControllerDecoratorBase>, IRequiresPreProcessing
    {
        public const string IDENTIFIER = "Intent.Messaging.LegacyCodeBasedDsl.Subscriber.WebApiEventConsumer";
        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IServiceModel _serviceModel;
        private IOperationModel _operationModel;
        private IEnumerable<WebApiControllerDecoratorBase> _decorators;

        public WebApiEventConsumerServiceTemplate(IProject project, SubscribingModel model, IApplicationEventDispatcher eventDispatcher)
            : base(IDENTIFIER, project, model)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        private IServiceModel GetServiceModel()
        {
            // Fabricate a ServiceModel for use by decorators.
            return _serviceModel ?? (_serviceModel = new ServiceModel(
                id: Guid.Empty.ToString(),
                name: ClassName,
                application: new Application(
                    id: Project.Application.Id,
                    name: Project.Application.ApplicationName),
                parentFolder: null,
                operations: new[]
                {
                    _operationModel = new OperationModel(
                        id: Guid.Empty.ToString(),
                        name: "ConsumeMessage",
                        parameters: new List<IOperationParameterModel>
                        {
                            new OperationParameterModel(
                                id: Guid.Empty.ToString(),
                                name: "content",
                                typeReference: new ClassTypeReference(
                                    id: Guid.Empty.ToString(),
                                    name: "string",
                                    isNullable: false,
                                    isCollection: false,
                                    folder: null,
                                    stereotypes: new IStereotype[0],
                                    comment: null),
                                stereotypes: new IStereotype[0],
                                comment: null)
                        },
                        returnType: null,
                        stereotypes: new IStereotype[0],
                        comment: null),
                },
                stereotypes: new IStereotype[0],
                comment: null));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "EventConsumerServiceController",
                fileExtension: "cs",
                defaultLocationInProject: "Controllers\\Generated",
                className: "MessageConsumingServiceController",
                @namespace: "${Project.ProjectName}.Controllers.Generated");
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

        public string DeclarePrivateVariables()
        {
            return GetDecorators().Aggregate(x => x.DeclarePrivateVariables(GetServiceModel()));
        }

        public string ConstructorParams()
        {
            return GetDecorators().Aggregate(x => x.ConstructorParams(GetServiceModel()));
        }

        public string ConstructorInit()
        {
            return GetDecorators().Aggregate(x => x.ConstructorInit(GetServiceModel()));
        }

        public string BeginOperation()
        {
            return GetDecorators().Aggregate(x => x.BeginOperation(GetServiceModel(), _operationModel));
        }

        public string BeforeTransaction()
        {
            return GetDecorators().Aggregate(x => x.BeforeTransaction(GetServiceModel(), _operationModel));
        }

        public string BeforeCallToAppLayer()
        {
            return GetDecorators().Aggregate(x => x.BeforeCallToAppLayer(GetServiceModel(), _operationModel));
        }

        public string AfterCallToAppLayer()
        {
            return GetDecorators().Aggregate(x => x.AfterCallToAppLayer(GetServiceModel(), _operationModel));
        }

        public string AfterTransaction()
        {
            return GetDecorators().Aggregate(x => x.AfterTransaction(GetServiceModel(), _operationModel));
        }

        public string OnExceptionCaught()
        {
            var onExceptionCaught = GetDecorators().Aggregate(x => x.OnExceptionCaught(GetServiceModel(), _operationModel));

            if (GetDecorators().Any(x => x.HandlesCaughtException()))
            {
                return onExceptionCaught;
            }

            return onExceptionCaught + @"
                throw;";
        }

        public string OnDispose()
        {
            return GetDecorators().Aggregate(x => x.OnDispose(GetServiceModel()));
        }

        public IEnumerable<WebApiControllerDecoratorBase> GetDecorators()
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
