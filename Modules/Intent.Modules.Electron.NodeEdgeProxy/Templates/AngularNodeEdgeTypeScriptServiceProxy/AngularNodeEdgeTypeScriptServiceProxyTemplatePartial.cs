using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Hosting;
using Intent.MetaModel.Service;
using Intent.Packages.Constants;
using Intent.Packages.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Electron.NodeEdgeProxy.Templates.AngularNodeEdgeTypeScriptServiceProxy
{
    partial class AngularNodeEdgeTypeScriptServiceProxyTemplate : IntentProjectItemTemplateBase<ServiceModel>, ITemplate, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.AngularNodeEdgeTypeScriptServiceProxy";

        private readonly HostingConfigModel _hostingConfig;
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public AngularNodeEdgeTypeScriptServiceProxyTemplate(ServiceModel model, HostingConfigModel hostingConfig, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, model)
        {
            _hostingConfig = hostingConfig;
            _eventDispatcher = eventDispatcher;
            var recevingProxyProject = project.Application.FindProjectWithTemplateInstance(NodeEdgeCsharpReceivingProxyTemplate.Identifier, model);
            AssemblyName = recevingProxyProject.Name;
        }

        public string Namespace => "App.Proxies";
        public string SolutionName => Project.Application.SolutionName;
        public string ApplicationName => Project.Application.ApplicationName;
        public string ApiBasePathConfigKey => $"{Project.Application.SolutionName}_{Project.ApplicationName()}_api_basepath".ToLower();
        public string AssemblyName { get; }
        public string TypeName => $"{AssemblyName}.{Model.Name}NodeEdgeProxy";

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\Proxies\Generated"
                );
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                { "Key", ApiBasePathConfigKey },
                { "Value", _hostingConfig?.GetBaseUrl() ?? "" }
            });
        }

        private static string GetMethodDefinitionParameters(IOperationModel operation)
        {
            if (!operation.Parameters.Any())
            {
                return "";
            }
            return operation.Parameters
                .Select(x => $"{LowerFirst(x.Name)}: {ConvertType(x.TypeReference.Name)}{(x.TypeReference.IsCollection ? "[]" : string.Empty)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        //private static string GetMethodCallParameters(IOperationModel operation)
        //{
        //    return operation.Parameters
        //        .Select(x => $"{LowerFirst(x.Name)}: {LowerFirst(x.Name)}")
        //        .Aggregate((x, y) => $"{x}, {y}");
        //}

        private static string GetReturnType(IOperationModel o)
        {
            if (o.ReturnType.TypeReference.Name.StartsWith("PagedResult"))
            {
                return $"Contracts.PagedResultDTO<{ConvertType(o.ReturnType.TypeReference.Name)}>";
            }

            if (o.ReturnType.TypeReference.IsCollection)
            {
                return $"{ConvertType(o.ReturnType.TypeReference.Name)}[]";
            }

            return $"{ConvertType(o.ReturnType.TypeReference.Name)}";
        }

        // TODO JL: This should work off GBs new system of stereotypes for mapping the types
        private static string ConvertType(string propertyType)
        {
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
                default:
                    {
                        if (propertyType.IndexOf("Contracts") != -1)
                        {
                            propertyType = propertyType.Substring(propertyType.IndexOf("Contracts"));
                        }
                        else
                        {
                            propertyType = "Contracts." + propertyType.Split('.')[propertyType.Split('.').Length - 1];
                        }

                        break;
                    }
            }

            return propertyType;
        }

        private static string LowerFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
