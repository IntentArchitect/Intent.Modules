using Intent.MetaModel.Common;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.AspNet.WebApi.Templates.Controller
{
    partial class WebApiControllerTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasTemplateDependencies, IHasAssemblyDependencies, IHasNugetDependencies, IHasDecorators<DistributionDecoratorBase>, IDeclareUsings
    {
        public const string Identifier = "Intent.WebApi.Controller";
        private IEnumerable<DistributionDecoratorBase> _decorators;

        public WebApiControllerTemplate(IProject project, IServiceModel model)
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
                TemplateDependancy.OnTemplate(ServiceContractTemplate.Identifier)
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
                defaultLocationInProject: "Controllers\\Generated",
                className: "${Model.Name}Controller",
                @namespace: "${Project.Name}"
                );
        }

        public string GetServiceInterfaceName()
        {
            var serviceContractTemplate = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<ServiceModel>(ServiceContractTemplate.Identifier, x => x.Id == Model.Id));
            return NormalizeNamespace($"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}");
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            if (typeInfo.IsCollection)
            {
                result = "List<" + result + ">";
            }
            return result;
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

        public IEnumerable<string> PayloadPropertyDecorators(IOperationParameterModel parameter)
        {
            return GetDecorators().SelectMany(x => x.PayloadPropertyDecorators(parameter));
        }

        public string BeginOperation(IOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeginOperation(Model, operation));
        }

        public string BeforeTransaction(IOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeTransaction(Model, operation));
        }

        public string BeforeCallToAppLayer(IOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.BeforeCallToAppLayer(Model, operation));
        }

        public string AfterCallToAppLayer(IOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.AfterCallToAppLayer(Model, operation));
        }

        public string AfterTransaction(IOperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.AfterTransaction(Model, operation));
        }

        public string OnExceptionCaught(IOperationModel operation)
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

        public IEnumerable<DistributionDecoratorBase> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        private string GetSecurityAttribute(IOperationModel o)
        {
            if (o.HasStereotype("Secured") || Model.HasStereotype("Secured"))
            {
                var roles = o.GetStereotypeProperty<string>("Secured", "Roles");
                return string.IsNullOrWhiteSpace(roles)
                    ? "[Authorize]" 
                    : $"[Authorize(Roles = \"{roles}\")]";
            }
            return "[AllowAnonymous]";
        }

        private string GetOperationCallParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"payload.{x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(IOperationModel o)
        {
            if (o.ReturnType == null)
            {
                return "void";
            }
            return GetTypeName(o.ReturnType.TypeReference);
        }
    }
}
