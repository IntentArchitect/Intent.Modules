using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts.Clients;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface;

namespace Intent.Modules.HttpServiceProxy.Templates.Proxy
{
    partial class WebApiClientServiceProxyTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasNugetDependencies, IHasAssemblyDependencies
    {
        public const string IDENTIFIER = "Intent.HttpServiceProxy.Proxy";

        public WebApiClientServiceProxyTemplate(IProject project, IServiceModel model)
            : base(IDENTIFIER, project, model)
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
                fileName: $"{Model.Name}WebApiClientProxy",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated\ClientProxies",
                className: "${Model.Name}WebApiClientProxy",
                @namespace: "${Project.Name}"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
                {
                    NugetPackages.MicrosoftAspNetWebApiClient,
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http")
            };
        }

        private string ApplicationName => Model.Application.Name;

        private string GetOperationDefinitionParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }

            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationCallParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }

            return o.Parameters.Select(x => $"{x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(IOperationModel o)
        {
            if (o.ReturnType == null)
            {
                return "void";
            }

            return GetTypeName(o.ReturnType.TypeReference);
        }

        private string GetServiceInterfaceName()
        {
            var serviceContractTemplate = Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<ServiceModel>(TemplateIds.ClientServiceContract, x => x.Id == Model.Id));
            if (serviceContractTemplate == null)
            {
                return $"I{Model.Name}";
            }

            return NormalizeNamespace($"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}");
        }

        private string GetHttpClientServiceInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(HttpClientServiceInterfaceTemplate.IDENTIFIER));
            if (template == null)
            {
                return "IHttpClientService";
            }
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
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

        private string GetReadAs(IOperationModel operation)
        {
            return $"ReadAsAsync<{GetOperationReturnType(operation)}>()";


            // UsesRawSignature not supported (yet?) in this new template, WebApi controllers aren't supporting it in the new template either
            //if (!operation.UsesRawSignature)
            //{
            //    return $"ReadAsAsync<{operation.ReturnType.FullName}>()";
            //}

            //if (operation.ReturnType.FullName.Equals(typeof(byte[]).Name, StringComparison.InvariantCultureIgnoreCase) ||
            //    operation.ReturnType.FullName.Equals(typeof(byte[]).FullName, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return "ReadAsByteArrayAsync()";
            //}

            //return $"ReadAsAsync<{operation.ReturnType.FullName}>()";
        }
    }
}
