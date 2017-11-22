using Intent.Modules.Application.Contracts.Legacy.DTO;
using Intent.Modules.Application.Contracts.Legacy.ServiceContract;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Class;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.AspNet.WebApi.Legacy.Controller
{
    partial class WebApiControllerTemplate : IntentRoslynProjectItemTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, IHasAssemblyDependencies, IHasNugetDependencies, IHasDecorators<IDistributionDecorator>, IDeclareUsings
    {
        public const string Identifier = "Intent.WebApi.Controller.Legacy";
        private IEnumerable<IDistributionDecorator> _decorators;

        public WebApiControllerTemplate(ServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public IEnumerable<string> DeclareUsings()
        {
            return GetDecorators().SelectMany(x => x.DeclareUsings(Model));
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DTOTemplate.Identifier),
                TemplateDependancy.OnTemplate(ServiceContractTemplate.Identifier),
            };
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new IAssemblyReference[]
            {
                new GacAssemblyReference("System.Transactions")
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftAspNetWebApi,
                NugetPackages.MicrosoftAspNetWebApiClient,
                NugetPackages.MicrosoftAspNetWebApiCore,
                NugetPackages.MicrosoftAspNetWebApiWebHost,
                NugetPackages.NewtonsoftJson,
                NugetPackages.IntentFrameworkWebApi,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: Model.Name + "Controller",
                fileExtension: "cs",
                defaultLocationInProject: @"Controllers\Generated",
                className: "${Model.Name}Controller",
                @namespace: "${Project.Name}"
                );
        }

        public string DeclarePrivateVariables()
        {
            return GetDecorators().Aggregate(x => x.DeclarePrivateVariables(Model));
        }

        public string ConstructorParams()
        {
            return GetDecorators().Aggregate(x => x.ConstructorParams(Model));
        }

        public string ConstructorInit()
        {
            return GetDecorators().Aggregate(x => x.ConstructorInit(Model));
        }

        public IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter)
        {
            return GetDecorators().SelectMany(x => x.PayloadPropertyDecorators(parameter));
        }

        public string BeginOperation(ServiceOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeginOperation(Model, operation));
        }

        public string BeforeTransaction(ServiceOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeTransaction(Model, operation));
        }

        public string BeforeCallToAppLayer(ServiceOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeCallToAppLayer(Model, operation));
        }

        public string AfterCallToAppLayer(ServiceOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.AfterCallToAppLayer(Model, operation));
        }

        public string AfterTransaction(ServiceOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.AfterTransaction(Model, operation));
        }

        public string OnExceptionCaught(ServiceOperationModel operation)
        {
            var onExceptionCaught = GetDecorators().Aggregate(x => x.OnExceptionCaught(Model, operation));

            if (GetDecorators().Any(x => x.HandlesCaughtException()))
            {
                return onExceptionCaught;
            }

            return onExceptionCaught + @"
                throw;";
        }

        public string ClassMethods()
        {
            return GetDecorators().Aggregate(x => x.ClassMethods(Model));
        }

        public IEnumerable<IDistributionDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private static string GetMethodReturnType(ServiceOperationModel operation)
        {
            return ReturnsHttpResponseMessage(operation)
                ? "HttpResponseMessage"
                : operation.ReturnType.FullName;
        }

        private static bool ReturnsHttpResponseMessage(ServiceOperationModel operation)
        {
            return operation.HasReturnType() && operation.UsesRawSignature;
        }

        private static string GetHttpResponseMessageContent(ServiceOperationModel operation)
        {
            if (operation.ReturnType.FullName.Equals(typeof(byte[]).Name, StringComparison.InvariantCultureIgnoreCase) ||
                operation.ReturnType.FullName.Equals(typeof(byte[]).FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                return "new StreamContent(new MemoryStream(appServiceResult))";
            }

            if (operation.ReturnType.FullName.Equals(typeof(string).Name, StringComparison.InvariantCultureIgnoreCase) ||
                operation.ReturnType.FullName.Equals(typeof(string).FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                return "new StringContent(appServiceResult)";
            }

            throw new InvalidOperationException($"Unknown: {operation.ReturnType.FullName}");
        }
    }
}
