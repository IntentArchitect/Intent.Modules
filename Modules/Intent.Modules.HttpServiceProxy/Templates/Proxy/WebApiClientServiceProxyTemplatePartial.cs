using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.MetaModel.Service;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.HttpServiceProxy.Templates.Proxy
{
    partial class WebApiClientServiceProxyTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasNugetDependencies, IHasAssemblyDependencies, IPostTemplateCreation
    {
        public const string IDENTIFIER = "Intent.HttpServiceProxy.Proxy";
        public const string SERVICE_CONTRACT_TEMPLATE_ID_CONFIG_KEY = "ServiceContractTemplateId";
        public const string HTTP_CLIENT_SERVICE_INTERFACE_TEMPLATE_ID_CONFIG_KEY = "HttpClientServiceInterfaceTemplateId";
        public const string DTO_TEMPLATE_ID_CONFIG_KEY = "DtoTemplateId";

        private string _serviceContractTemplateId;
        private string _httpClientServiceInterfaceTemplateId;
        private string _dtoTemplateId;

        public WebApiClientServiceProxyTemplate(IProject project, IServiceModel model, string identifier = IDENTIFIER)
            : base(identifier, project, model)
        {
        }

        public void Created()
        {
            _serviceContractTemplateId = GetMetaData().CustomMetaData[SERVICE_CONTRACT_TEMPLATE_ID_CONFIG_KEY];
            _httpClientServiceInterfaceTemplateId = GetMetaData().CustomMetaData[HTTP_CLIENT_SERVICE_INTERFACE_TEMPLATE_ID_CONFIG_KEY];
            _dtoTemplateId = GetMetaData().CustomMetaData[DTO_TEMPLATE_ID_CONFIG_KEY];
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
                defaultLocationInProject: @"Generated/ClientProxies",
                className: "${Model.Name}WebApiClientProxy",
                @namespace: "${Project.Name}");
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
            var serviceContractTemplate = Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<ServiceModel>(_serviceContractTemplateId, x => x.Id == Model.Id));
            if (serviceContractTemplate == null)
            {
                Logging.Log.Warning($"Could not find template with ID [{_serviceContractTemplateId}] " +
                                    $"as configured for the [{SERVICE_CONTRACT_TEMPLATE_ID_CONFIG_KEY}] " +
                                    $"setting on the [{Id}] template.");

                return $"I{Model.Name}";
            }

            return NormalizeNamespace($"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}");
        }

        private string GetHttpClientServiceInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(_httpClientServiceInterfaceTemplateId));
            if (template == null)
            {
                Logging.Log.Warning($"Could not find template with ID [{_httpClientServiceInterfaceTemplateId}] " +
                                    $"as configured for the [{HTTP_CLIENT_SERVICE_INTERFACE_TEMPLATE_ID_CONFIG_KEY}] " +
                                    $"setting on the [{Id}] template.");

                return "IHttpClientService";
            }

            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            if (typeInfo.Type != ReferenceType.ClassType)
            {
                return NormalizeNamespace(typeInfo.IsCollection
                    ? $"List<{Types.Get(typeInfo)}>"
                    : Types.Get(typeInfo));
            }

            var templateInstance = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<IDTOModel>(_dtoTemplateId, x => x.Id == typeInfo.Id));
            if (templateInstance == null)
            {
                Logging.Log.Warning($"Could not find template with ID [{_dtoTemplateId}] " +
                                    $"as configured for the [{DTO_TEMPLATE_ID_CONFIG_KEY}] " +
                                    $"setting on the [{Id}] template.");

                return NormalizeNamespace(typeInfo.IsCollection
                    ? $"List<{Types.Get(typeInfo)}>"
                    : Types.Get(typeInfo));
            }

            var typeName = $"{templateInstance.Namespace}.{templateInstance.ClassName}";

            return NormalizeNamespace(typeInfo.IsCollection
                ? $"List<{typeName}>"
                : typeName);
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
