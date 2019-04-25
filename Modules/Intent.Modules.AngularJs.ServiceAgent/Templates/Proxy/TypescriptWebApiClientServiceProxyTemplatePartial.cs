using Intent.MetaModel.Hosting;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.MetaModels.Common;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AngularJs.ServiceAgent.Templates.Proxy
{
    partial class TypescriptWebApiClientServiceProxyTemplate : IntentTypescriptProjectItemTemplateBase<ServiceModel>, ITemplate, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.AngularJs.ServiceAgent.Proxy";

        private readonly HostingConfigModel _hostingConfig;
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public TypescriptWebApiClientServiceProxyTemplate(ServiceModel model, HostingConfigModel hostingConfig, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, model)
        {
            _hostingConfig = hostingConfig ?? new HostingConfigModel() { UseSsl = true, SslPort = "44399" };
            _eventDispatcher = eventDispatcher;
        }

        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $"wwwroot/App/Proxies/Generated",
                className: "${Model.Name}Proxy",
                @namespace: "App.Proxies"
                );
        }

        public string AngularModule
        {
            get
            {
                string angularModule;
                if (GetMetaData().CustomMetaData.TryGetValue("AngularModule", out angularModule))
                {
                    return angularModule;
                }
                return "App"; // Default Angular Module
            }
        }

        public string SolutionName => Project.Application.SolutionName;
        public string ApplicationName => Project.Application.ApplicationName;
        public string ApiBasePathConfigKey => $"{Project.Application.SolutionName}_{Model.ApplicationName}_api_basepath".ToLower();
        public string ContractNamespace => Model.ApplicationName == Project.ApplicationName().Replace("_Client", "") ? "Contracts" : $"Contracts.{Model.ApplicationName}";

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                { "Key", ApiBasePathConfigKey },
                { "Value", _hostingConfig.GetBaseUrl() }
            });
        }

        string GetReturnType(ServiceOperationModel o)
        {
            if (o.ReturnType.TypeName.StartsWith("PagedResult"))
            {
                return "Contracts.PagedResultDTO<" + ConvertType(o.ReturnType.GenericArguments[0]) + ">";
            }
            else if (o.ReturnType.TypeName.StartsWith("IList") || o.ReturnType.TypeName.StartsWith("List"))
            {
                return ConvertType(o.ReturnType.GenericArguments[0]) + "[]";
            }
            if (o.ReturnType.IsGenericType && o.ReturnType.TypeName.Contains("Nullable"))
            {
                return ConvertType(o.ReturnType.GenericArguments[0]);
            }
            else if (o.ReturnType.IsGenericType && o.ReturnType.TypeName.Contains("Dictionary"))
            {
                return "any[]";
            }
            else
            {
                return ConvertType(o.ReturnType);
            }
        }

        string ConvertType(TypeModel typeModel)
        {
            var propertyType = typeModel.FullName;
            if (typeModel.IsEnum)
            {
                return "Contracts." + typeModel.TypeName;
            }
            switch (propertyType)
            {
                case "byte[]":
                case "Byte[]":
                case "System.Byte[]":
                    propertyType = "Array<number>";
                    break;
                case "DateTime":
                case "DateTime?":
                case "System.DateTime":
                case "System.DateTime?":
                case "System.Nullable<System.DateTime>":
                    propertyType = "Date";
                    break;
                case "string":
                case "String":
                case "System.String":
                case "Guid":
                case "Guid?":
                case "System.Guid":
                case "System.Guid?":
                case "System.Nullable<System.Guid>":
                    propertyType = "string";
                    break;
                case "int":
                case "Int32":
                case "Int32?":
                case "System.Int32":
                case "System.Int32?":
                case "System.Nullable<System.Int32>":
                case "decimal":
                case "Decimal":
                case "Decimal?":
                case "System.Decimal":
                case "System.Decimal?":
                case "System.Nullable<System.Decimal>":
                case "System.TimeSpan":
                case "System.TimeSpan?":
                case "System.Nullable<System.TimeSpan>":
                    propertyType = "number";
                    break;
                case "bool":
                case "Boolean":
                case "System.Boolean":
                    propertyType = "boolean";
                    break;
                case "object":
                case "Object":
                case "System.Object":
                    propertyType = "any";
                    break;
                default:
                    {
                        if (propertyType.IndexOf("Contracts") != -1)
                        {
                            propertyType = propertyType.Substring(propertyType.IndexOf("Contracts"));
                        }
                        else
                        {
                            propertyType = ContractNamespace + "." + propertyType.Split('.')[propertyType.Split('.').Length - 1];
                        }

                        break;
                    }
            }

            return propertyType;
        }
    }
}
